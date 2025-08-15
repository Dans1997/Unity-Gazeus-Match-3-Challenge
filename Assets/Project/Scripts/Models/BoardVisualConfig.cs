using System;
using Sirenix.Serialization;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct BoardVisualConfig
    {
        [OdinSerialize] public Vector2 CellSize { get; private set; }
        [OdinSerialize] public Vector2 Spacing { get; private set; }
    }
}