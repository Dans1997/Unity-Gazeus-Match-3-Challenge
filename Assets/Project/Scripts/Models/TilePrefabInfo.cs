using System;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Sirenix.Serialization;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct TilePrefabInfo
    {
        [OdinSerialize] public TileKey TileKey { get; private set; }
        [OdinSerialize] public GameObject Prefab { get; private set; }
        
        public TilePrefabInfo(TileKey tileKey, GameObject prefab)
        {
            TileKey = tileKey;
            Prefab = prefab;
        }
    }
}