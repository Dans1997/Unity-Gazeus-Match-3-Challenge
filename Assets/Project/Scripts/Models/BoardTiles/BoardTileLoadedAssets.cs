using System;
using Sirenix.Serialization;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Models.BoardTiles
{
    [Serializable]
    public class BoardTileLoadedAssets
    {
        [OdinSerialize] public Sprite TileSprite { get; }
        [OdinSerialize] public GameObject DestructionPrefab { get; }

        public BoardTileLoadedAssets(Sprite tileSprite, GameObject destructionPrefab = null)
        {
            TileSprite = tileSprite ?? throw new ArgumentNullException(nameof(tileSprite));
            DestructionPrefab = destructionPrefab;
        }
    }
}