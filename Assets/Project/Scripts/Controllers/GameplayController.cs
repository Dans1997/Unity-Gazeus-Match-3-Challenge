using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.DesafioMatch3.Views;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class GameplayController : IGameplayController
    {
        public GameplayInfo GameplayInfo { get; private set; }
        public IAssetProvider AssetProvider { get; private set; }
        public IGameplayService GameplayService { get; private set; }
        public BoardView BoardView { get; private set; }
        public IBoardCellView SelectedCellView { get; private set; }
        public bool IsAnimating { get; private set; }
        public int SelectedX => SelectedCellView?.Position.x ?? -1;
        public int SelectedY => SelectedCellView?.Position.y ?? -1;
        
        public GameplayController(GameplayInfo gameplayInfo, IAssetProvider assetProvider,
            IGameplayService gameplayService)
        {
            GameplayInfo = gameplayInfo;
            AssetProvider = assetProvider;
            GameplayService = gameplayService;
        } 
        
        public async UniTask Initialize()
        {
            BoardView = await AssetProvider.InstantiateAsync<BoardView>(GameplayInfo.GameplayViewPrefabKey);
            var boardCellViewPrefab = await AssetProvider.LoadAssetAsync<GameObject>(GameplayInfo.BoardCellViewPrefabKey);
            var board = GameplayService.StartGame(GameplayInfo);
            var preloadedTiles = await LoadTilesAsync(GameplayInfo.AvailableTileKeys);
            
            BoardView.ConfigureBoardVisuals(GameplayInfo.BoardVisualConfig, board[0].Count);
            BoardView.CreateBoard(board, boardCellViewPrefab.GetComponent<IBoardCellView>(), preloadedTiles);
            BoardView.TileClicked += OnTileClick;
        }
        
        public void Dispose()
        {
            if (BoardView == null) return;
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

            if (Mathf.Abs(SelectedX - x) + Mathf.Abs(SelectedY - y) > 1)
            {
                DeselectBoardCellView();
                return;
            }
            
            SelectedCellView?.SetSelected(false);
            
            IsAnimating = true;
            BoardView.SwapTiles(SelectedX, SelectedY, x, y).onComplete += () =>
            {
                var isValid = GameplayService.IsValidMovement(SelectedX, SelectedY, x, y);
                if (isValid)
                {
                    var swapResult = GameplayService.SwapTile(SelectedX, SelectedY, x, y);
                    AnimateBoard(swapResult, 0, () => IsAnimating = false);
                }
                else
                {
                    BoardView.SwapTiles(x, y, SelectedX, SelectedY).onComplete += () => IsAnimating = false;
                }

                DeselectBoardCellView();
            };
        }

        private void SelectBoardCellView(IBoardCellView boardCellView)
        {
            SelectedCellView = boardCellView;
            SelectedCellView?.SetSelected(true);
            Debug.Log($"[GaneplayController] Tile {boardCellView.Position} selected");
        }

        private void DeselectBoardCellView()
        {
            SelectedCellView?.SetSelected(false);
            SelectedCellView = null;
        }

        private async UniTask<TilePrefabInfo[]> LoadTilesAsync(TileKey[] keys)
        {
            var maxIndex = keys.Max(k => (int)k);
            var entries = new TilePrefabInfo[maxIndex + 1];
            var prefabs = await AssetProvider.LoadAssetsAsync<TileKey, GameObject>(keys);
            
            for (var i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                entries[(int)key] = new TilePrefabInfo(key, prefabs[i]);
            }

            return entries;
        }
    }
}
