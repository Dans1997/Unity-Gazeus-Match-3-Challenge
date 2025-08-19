using System;
using System.Collections.Generic;
using DG.Tweening;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Models.BoardTiles;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles
{
    public interface IGameplayView : IView
    {
        event Action<IBoardCellView> TileClicked;
        public event Action<IBoardTileView> TileDestroyed;
        
        void ConfigureBoardVisuals(BoardVisualConfig config, BoardTileLoadedAssets[] boardTileLoadedAssets,
            IBoardCellView boardCellViewPrefab, IBoardTileView boardTileViewPrefab, int constraintCount);
        void BuildBoardVisuals(IReadOnlyList<IReadOnlyList<BoardTileInfo>> boardStateBoardTiles);
        Tween CreateTile(IReadOnlyList<AddedTileInfo> addedTiles);
        Tween DestroyTiles(IReadOnlyList<Vector2Int> matchedPosition);
        Tween MoveTiles(IReadOnlyList<MovedTileInfo> movedTiles);
        Tween SwapTiles(int fromX, int fromY, int toX, int toY);
        Tween UpdateScore(BoardSequenceScoreInfo boardSequenceScoreInfo, float duration = 0.25f);
    }
}