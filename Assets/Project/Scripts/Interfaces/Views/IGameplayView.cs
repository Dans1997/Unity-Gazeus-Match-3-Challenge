using System;
using System.Collections.Generic;
using DG.Tweening;
using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles
{
    public interface IGameplayView : IView
    {
        event Action<IBoardCellView> TileClicked;
        
        void ConfigureBoardVisuals(BoardVisualConfig gameplayInfoBoardVisualConfig, int count);
        void BuildBoardVisuals(IReadOnlyList<IReadOnlyList<TileInfo>> board, IBoardCellView getComponent,
            TilePrefabInfo[] preloadedTiles);
        Tween CreateTile(List<AddedTileInfo> addedTiles);
        Tween DestroyTiles(List<Vector2Int> matchedPosition);
        Tween MoveTiles(List<MovedTileInfo> movedTiles);
        Tween SwapTiles(int fromX, int fromY, int toX, int toY);
        Tween UpdateScore(BoardSequenceScoreInfo boardSequenceScoreInfo, float duration = 0.25f);
    }
}