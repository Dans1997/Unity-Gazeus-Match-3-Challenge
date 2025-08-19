using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Models.BoardTiles;
using Gazeus.Match3Challenge.Project.Scripts.Models.BoardTiles;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles
{
    public interface IGameplayView : IView
    {
        event Action<IBoardCellView> TileClicked;
        public event Action<IBoardTileView> TileDestroyed;
        public event Action HintButtonClicked;
        public event Action QuitGameButtonClicked;
        
        void ConfigureBoardVisuals(BoardVisualConfig config, BoardTileLoadedAssets[] boardTileLoadedAssets,
            IBoardCellView boardCellViewPrefab, IBoardTileView boardTileViewPrefab, int constraintCount);
        UniTask BuildBoardVisuals(IReadOnlyList<IReadOnlyList<BoardTileInfo>> boardStateBoardTiles);
        Tween CreateTile(IReadOnlyList<AddedTileInfo> addedTiles);
        Tween DestroyTiles(IReadOnlyList<Vector2Int> matchedPosition);
        Tween MoveTiles(IReadOnlyList<MovedTileInfo> movedTiles);
        Tween SwapTiles(int fromX, int fromY, int toX, int toY);
        Tween UpdateScore(BoardSequenceScoreInfo boardSequenceScoreInfo, float duration = 0.25f);
        void UpdateTime(float timeLeft);
        void HighlightBoardMatch(ValidTileMoveInfo validMoveInfo, float duration = 3f);
    }
}