using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Views;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class GameplayController : IGameplayController
    {
        public GameplayInfo GameplayInfo { get; private set; }
        public IAssetProvider AssetProvider { get; private set; }
        public IGameplayService GameplayService { get; private set; }
        public BoardView BoardView { get; private set; }
        public bool IsAnimating { get; private set; }
        public int SelectedX { get; private set; }
        public int SelectedY { get; private set; }
        
        public GameplayController(GameplayInfo gameplayInfo, IAssetProvider assetProvider,
            IGameplayService gameplayService)
        {
            GameplayInfo = gameplayInfo;
            AssetProvider = assetProvider;
            GameplayService = gameplayService;
        } 
        
        public async UniTask Initialize()
        {
            BoardView = await AssetProvider.InstantiateAsync<BoardView>(GameplayInfo.GameplayViewPrefabKey.ToString());
            var board = GameplayService.StartGame(GameplayInfo.BoardWidth, GameplayInfo.BoardHeight);
            
            BoardView.CreateBoard(board);
            BoardView.TileClicked += OnTileClick;
        }
        
        public void Dispose()
        {
            BoardView.TileClicked -= OnTileClick;
            AssetProvider.Release(BoardView.gameObject);
            BoardView = null;
        }

        private void AnimateBoard(List<BoardSequence> boardSequences, int index, Action onComplete)
        {
            var boardSequence = boardSequences[index];

            var sequence = DOTween.Sequence();
            sequence.Append(BoardView.DestroyTiles(boardSequence.MatchedPosition));
            sequence.Append(BoardView.MoveTiles(boardSequence.MovedTiles));
            sequence.Append(BoardView.CreateTile(boardSequence.AddedTiles));

            index += 1;
            if (index < boardSequences.Count)
            {
                sequence.onComplete += () => AnimateBoard(boardSequences, index, onComplete);
            }
            else
            {
                sequence.onComplete += () => onComplete();
            }
        }

        private void OnTileClick(int x, int y)
        {
            if (IsAnimating) return;

            if (SelectedX > -1 && SelectedY > -1)
            {
                if (Mathf.Abs(SelectedX - x) + Mathf.Abs(SelectedY - y) > 1)
                {
                    SelectedX = -1;
                    SelectedY = -1;
                }
                else
                {
                    IsAnimating = true;
                    BoardView.SwapTiles(SelectedX, SelectedY, x, y).onComplete += () =>
                    {
                        bool isValid = GameplayService.IsValidMovement(SelectedX, SelectedY, x, y);
                        if (isValid)
                        {
                            List<BoardSequence> swapResult = GameplayService.SwapTile(SelectedX, SelectedY, x, y);
                            AnimateBoard(swapResult, 0, () => IsAnimating = false);
                        }
                        else
                        {
                            BoardView.SwapTiles(x, y, SelectedX, SelectedY).onComplete += () => IsAnimating = false;
                        }
                        SelectedX = -1;
                        SelectedY = -1;
                    };
                }
            }
            else
            {
                SelectedX = x;
                SelectedY = y;
            }
        }
    }
}
