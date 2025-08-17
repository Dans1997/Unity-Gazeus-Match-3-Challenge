using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gazeus.DesafioMatch3.Core.Services;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.DesafioMatch3.Views;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Scores;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using Gazeus.Match3Challenge.Project.Scripts.Core.Rules;
using Gazeus.Match3Challenge.Project.Scripts.Core.Services;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class GameplayController : IGameplayController
    {
        public event Action GameStarted;
        public event Action TileClicked;
        public event Action TileSelected;
        public event Action TileSwapped;
        public event Action ScoreUpdated;
        public event Action<GameEndResults> GameEnded;
        
        public GameplayInfo GameplayInfo { get; private set; }
        public IAssetLoadService AssetLoadService { get; private set; }
        public IBoardService BoardService { get; private set; }
        public IScoreService ScoreService { get; private set; }
        public IGameplayView GameplayView { get; private set; }
        public IBoardCellView SelectedCellView { get; private set; }
        public IGameEndRule[] GameEndRules { get; private set; }
        public bool IsAnimating { get; private set; }
        public int SelectedX => SelectedCellView?.Position.x ?? -1;
        public int SelectedY => SelectedCellView?.Position.y ?? -1;
        
        private GameObject boardCellViewPrefab;
        private TilePrefabInfo[] preloadedTiles;
        private float gameStartTime;

        public GameplayController(GameplayInfo gameplayInfo, IAssetLoadService assetLoadService)
        {
            GameplayInfo = gameplayInfo;
            AssetLoadService = assetLoadService;
            BoardService = new BoardService(gameplayInfo);
            ScoreService = new ScoreService();
            
            var gameEndRuleFactory = new GameEndRuleFactory();
            GameEndRules = GameplayInfo.GameEndRules    
                .Select(config => gameEndRuleFactory.Create(config))
                .ToArray();
        } 
        
        public async UniTask Initialize()
        {
            GameplayView = await AssetLoadService.InstantiateAsync<GameplayView>(GameplayInfo.GameplayViewPrefabKey);
            boardCellViewPrefab = await AssetLoadService.LoadAssetAsync<GameObject>(GameplayInfo.BoardCellViewPrefabKey);
            preloadedTiles = await LoadTilesAsync(GameplayInfo.AvailableTileKeys);
            
            BoardService.CreateBoard();
            GameplayView.ConfigureBoardVisuals(GameplayInfo.BoardVisualConfig, BoardService.BoardState.BoardTiles[0].Count);
            GameplayView.BuildBoardVisuals(BoardService.BoardState.BoardTiles, boardCellViewPrefab.GetComponent<IBoardCellView>(), preloadedTiles);
        }
        
        public void Dispose()
        {
            if (GameplayView == null) return;
            GameplayView.TileClicked -= OnTileClick;
            AssetLoadService.Release(GameplayView.Transform.gameObject);
            GameplayView = null;
            
            ScoreService.ScoreUpdated -= OnScoreUpdated;
        }

        public void StartGame()
        {
            gameStartTime = Time.time;
            GameplayView.TileClicked += OnTileClick;
            ScoreService.ScoreUpdated += OnScoreUpdated;
            
            foreach (var asyncRule in GameEndRules.OfType<IAsyncGameEndRule>())
            {
                asyncRule.StartAsync(BoardService, ScoreService, OnAsyncRuleTriggered).Forget(); 
            }
            
            GameStarted?.Invoke();
        }

        private void OnScoreUpdated(int newScore)
        {
            ScoreUpdated?.Invoke();
        }

        private void AnimateBoard(IReadOnlyList<BoardSequence> boardSequences, int index, Action onComplete)
        {
            var boardSequence = boardSequences[index];
            var boardSequenceScoreInfo = ScoreService.CalculateSequenceScore(boardSequence, index);
                
            var sequence = DOTween.Sequence();
            sequence.Append(GameplayView.DestroyTiles(boardSequence.MatchedPosition));
            sequence.Append(GameplayView.MoveTiles(boardSequence.MovedTiles));
            sequence.Append(GameplayView.CreateTile(boardSequence.AddedTiles));
            sequence.Append(GameplayView.UpdateScore(boardSequenceScoreInfo));
            
            Debug.Log($"[GameplayController] Animating sequence: \n{boardSequence}");

            index += 1;
            if (index < boardSequences.Count)
            {
                sequence.onComplete += () => AnimateBoard(boardSequences, index, onComplete);
                return;
            }

            sequence.onComplete += () => onComplete();
        }

        private void OnTileClick(IBoardCellView clickedCellView)
        {
            if (IsAnimating) return;

            var x = clickedCellView.Position.x;
            var y = clickedCellView.Position.y;
            
            if (SelectedCellView == null)
            {
                SelectBoardCellView(clickedCellView);
                return;
            }

            if (SelectedCellView == clickedCellView || Mathf.Abs(SelectedX - x) + Mathf.Abs(SelectedY - y) > 1)
            {
                DeselectBoardCellView();
                return;
            }
            
            SelectedCellView?.SetSelected(false);
            
            IsAnimating = true;
            GameplayView.SwapTiles(SelectedX, SelectedY, x, y).onComplete += () =>
            {
                var isValid = BoardService.IsValidMovement(SelectedX, SelectedY, x, y);
                if (isValid)
                {
                    var swapResult = BoardService.SwapTile(SelectedX, SelectedY, x, y);
                    AnimateBoard(swapResult.Sequences, 0, OnBoardAnimationEnded);
                }
                else
                {
                    GameplayView.SwapTiles(x, y, SelectedX, SelectedY).onComplete += () => IsAnimating = false;
                    TileSwapped?.Invoke();
                }

                DeselectBoardCellView();
            };
            
            TileSwapped?.Invoke();
        }

        private void SelectBoardCellView(IBoardCellView boardCellView)
        {
            SelectedCellView = boardCellView;
            SelectedCellView?.SetSelected(true);
            TileSelected?.Invoke();
            Debug.Log($"[GameplayController] Tile {boardCellView.Position} selected");
        }

        private void DeselectBoardCellView()
        {
            SelectedCellView?.SetSelected(false);
            SelectedCellView = null;
        }

        private void OnBoardAnimationEnded()
        {
            IsAnimating = false;
            CheckForGameEnd();
        }
        
        private void OnAsyncRuleTriggered(IGameEndRule triggeredEndRule)
        {
            if (IsAnimating) return;
            EndGame(new[] { triggeredEndRule });
        }
        
        private void CheckForGameEnd()
        {
            var triggeredRules = GameEndRules
                .Where(rule => rule.IsGameOver(BoardService, ScoreService))
                .ToList();

            if (!triggeredRules.Any()) return;
            
            EndGame(triggeredRules);
        }

        private void EndGame(IReadOnlyCollection<IGameEndRule> triggeredEndGameRules)
        {
            var gameEndResults = new GameEndResults
            {
                FinalScore = ScoreService.CurrentScore,
                FinalTimeInSeconds = Time.time - gameStartTime,
                TriggeredEndRules = triggeredEndGameRules
            };
            
            GameEnded?.Invoke(gameEndResults);
        }

        private async UniTask<TilePrefabInfo[]> LoadTilesAsync(TileKey[] keys)
        {
            var maxIndex = keys.Max(k => (int)k);
            var entries = new TilePrefabInfo[maxIndex + 1];
            var prefabs = await AssetLoadService.LoadAssetsAsync<TileKey, GameObject>(keys);
            
            for (var i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                entries[(int)key] = new TilePrefabInfo(key, prefabs[i]);
            }

            return entries;
        }
    }
}
