using System;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles;
using Sirenix.Serialization;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct BoardVisualConfig
    {
        [OdinSerialize] public Vector2 CellSize { get; private set; }
        [OdinSerialize] public Vector2 Spacing { get; private set; }
        [OdinSerialize] public string ScoreFormat { get; private set; }
    }
}