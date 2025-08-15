using System;
using DG.Tweening;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles
{
    public interface IBoardCellView
    {
        event Action<IBoardCellView> Clicked;

        Transform Transform { get; }
        Vector2Int Position { get; }
        
        void SetPosition(Vector2Int position);
        void SetSelected(bool selected);
        void SetTile(Transform tile);
        Tween AnimatedSetTile(Transform tile);
    }
}