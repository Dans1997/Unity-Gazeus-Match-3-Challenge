using System;
using DG.Tweening;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles
{
    public interface IBoardCellView : IView
    {
        event Action<IBoardCellView> Clicked;


        Vector2Int Position { get; }
        
        void SetPosition(Vector2Int position);
        void SetSelected(bool selected);
        void SetTile(Transform tile);
        Tween AnimatedSetTile(Transform tile);
    }
}