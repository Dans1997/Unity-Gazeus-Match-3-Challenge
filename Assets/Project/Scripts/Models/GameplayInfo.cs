using System;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Sirenix.Serialization;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct GameplayInfo
    {
        [OdinSerialize] public int BoardHeight { get; private set; }
        [OdinSerialize] public int BoardWidth { get; private set; }
        [OdinSerialize] public GameplayViewKey GameplayViewPrefabKey { get; private set; }
        [OdinSerialize] public BoardCellViewKey BoardCellViewPrefabKey { get; private set; }
        [OdinSerialize] public GameObject[] TilePrefabs { get; private set; }
    }
}