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
using UnityEngine.AddressableAssets;

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
            BoardView = await AssetProvider.InstantiateAsync<BoardView>(GameplayInfo.GameplayViewPrefabKey);
            var boardCellViewPrefab = await AssetProvider.LoadAssetAsync<GameObject>(GameplayInfo.BoardCellViewPrefabKey);
            var board = GameplayService.StartGame(GameplayInfo);
            var preloadedTiles = await LoadTilesAsync(GameplayInfo.AvailableTileKeys);
            
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

        private void OnTileClick(Vector2Int position)
        {
            if (IsAnimating) return;

            var x = position.x;
            var y = position.y;
            
            if (SelectedX <= -1 || SelectedY <= -1)
            {
                SelectedX = x;
                SelectedY = y;
                return;
            }

            if (Mathf.Abs(SelectedX - x) + Mathf.Abs(SelectedY - y) > 1)
            {
                SelectedX = -1;
                SelectedY = -1;
                return;
            }

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

                SelectedX = -1;
                SelectedY = -1;
            };
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
