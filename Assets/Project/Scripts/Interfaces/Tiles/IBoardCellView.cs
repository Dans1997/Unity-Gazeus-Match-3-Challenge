using System;
using DG.Tweening;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles
{
    public interface IBoardCellView
    {
        event Action<Vector2Int> Clicked;

        Transform Transform { get;  }
        
        void SetPosition(Vector2Int position);
        void SetTile(Transform tile);
        Tween AnimatedSetTile(Transform tile);
    }
}