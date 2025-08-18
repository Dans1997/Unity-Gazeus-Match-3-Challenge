using System;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public class BoardTileConfig
    {
        [OdinSerialize] public TileKey TileKey { get; private set; }
        [OdinSerialize] [PropertyRange(0, 100)] public int Weight { get; private set; } = 1;
        [OdinSerialize] public BoardTileDestructionConfig BoardTileDestructionConfig { get; private set; }
        [OdinSerialize] public string TileSpriteKey => $"{TileKey}Sprite";
    }
}