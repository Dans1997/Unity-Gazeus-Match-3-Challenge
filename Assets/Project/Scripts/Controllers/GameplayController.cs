using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gazeus.DesafioMatch3.Core.Services;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Models.BoardTiles;
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
            var boardCellViewPrefab = await AssetLoadService.LoadAssetAsync<GameObject>(GameplayInfo.BoardCellViewPrefabKey);
            var boardTileViewPrefab = await AssetLoadService.LoadAssetAsync<GameObject>(GameplayInfo.BoardTileViewPrefabKey);
            var boardTileLoadedAssets = await LoadBoardTileAssetsAsync(GameplayInfo.AvailableTileConfigs);
            
            BoardService.CreateBoard();
            
            GameplayView.ConfigureBoardVisuals
            (
                GameplayInfo.BoardVisualConfig, 
                boardTileLoadedAssets, 
                boardCellViewPrefab.GetComponent<IBoardCellView>(), 
                boardTileViewPrefab.GetComponent<IBoardTileView>(), 
                BoardService.BoardState.BoardWidth
            );
            
            foreach (var asyncRule in GameEndRules.OfType<IAsyncGameEndRule>())
            {
                if (asyncRule is TimerRule timerRule) // TODO: Assuming only one timer rule
                {
                    GameplayView.UpdateTime(timerRule.DurationInSeconds);
                    timerRule.TimeLeftUpdated += GameplayView.UpdateTime;
                }
            }
            
            await GameplayView.BuildBoardVisuals(BoardService.BoardState.BoardTiles);
        }
        
        public void StartGame()
        {
            gameStartTime = Time.time;
            GameplayView.TileClicked += OnTileClick;
            GameplayView.TileDestroyed += OnTileDestroyed;
            GameplayView.HintButtonClicked += OnHintButtonClicked;
            GameplayView.QuitGameButtonClicked += OnQuitGameButtonClicked;
            ScoreService.ScoreUpdated += OnScoreUpdated;
            
            foreach (var asyncRule in GameEndRules.OfType<IAsyncGameEndRule>())
            {
                asyncRule.StartAsync(BoardService, ScoreService, OnAsyncRuleTriggered).Forget(); 
            }
            
            GameStarted?.Invoke();
        }
        
        public void Dispose()
        {
            if (GameplayView == null) return;
            GameplayView.TileClicked -= OnTileClick;
            GameplayView.TileDestroyed -= OnTileDestroyed;
            GameplayView.HintButtonClicked -= OnHintButtonClicked;
            GameplayView.QuitGameButtonClicked -= OnQuitGameButtonClicked;
            ScoreService.ScoreUpdated -= OnScoreUpdated;
            
            foreach (var asyncRule in GameEndRules.OfType<IAsyncGameEndRule>())
            {
                if (asyncRule is TimerRule timerRule) 
                {
                    timerRule.TimeLeftUpdated -= GameplayView.UpdateTime;
                }
            }
            
            AssetLoadService.Release(GameplayView.Transform.gameObject);
            GameplayView = null;
        }

        private void AnimateBoard(IReadOnlyList<BoardSequence> boardSequences, int index, Action onComplete)
        {
            var boardSequence = boardSequences[index];
            var boardSequenceScoreInfo = ScoreService.CalculateSequenceScore(boardSequence, index);
                
            var sequence = DOTween.Sequence();
            sequence.Append(GameplayView.DestroyTiles(boardSequence.MatchedPositions));
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
                var isValid = BoardService.IsValidMove(SelectedX, SelectedY, x, y);
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
        
        private void OnHintButtonClicked()
        {
            BoardService.HasAnyValidMove(out var firstValidMove);
            GameplayView.HighlightBoardMatch(firstValidMove);
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
            var gameEndResults = new GameEndResults(ScoreService.CurrentScore, Time.time - gameStartTime,
                triggeredEndGameRules);
            
            GameEnded?.Invoke(gameEndResults);
        }
        
        private async UniTask<BoardTileLoadedAssets[]> LoadBoardTileAssetsAsync(BoardTileConfig[] boardTileConfigs)
        {
            var loadedAssets = new BoardTileLoadedAssets[boardTileConfigs.Length];

            var spriteKeys = boardTileConfigs.Select(c => c.TileSpriteKey).ToArray();
            var destructionKeys =
                boardTileConfigs.Select(c => c.BoardTileDestructionConfig.TileDestructionKey).ToArray();
            
            var tileSprite = await AssetLoadService.LoadAssetsAsync<Sprite>(spriteKeys);
            var destructionPrefab = await AssetLoadService.LoadAssetsAsync<TileDestructionKey, GameObject>(destructionKeys);

            for (var i = 0; i < loadedAssets.Length; i++)
            {
                loadedAssets[i] = new BoardTileLoadedAssets
                (
                    tileSprite[i],
                    destructionPrefab[i]
                );
            }
            
            return loadedAssets;
        }
        
        private void OnTileDestroyed(IBoardTileView boardTileView)
        {
            throw new NotImplementedException();
        }

        private void OnQuitGameButtonClicked() => EndGame(null);
        private void OnScoreUpdated(int newScore) => ScoreUpdated?.Invoke();
    }
}
