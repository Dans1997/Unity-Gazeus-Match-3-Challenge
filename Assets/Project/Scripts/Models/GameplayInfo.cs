using System;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Sirenix.Serialization;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct GameplayInfo
    {
        [OdinSerialize] public int BoardHeight { get; private set; }
        [OdinSerialize] public int BoardWidth { get; private set; }
        [OdinSerialize] public GameplayViewKey GameplayViewPrefabKey { get; private set; }
        [OdinSerialize] public GameOverViewKey GameOverViewPrefabKey { get; private set; }
        [OdinSerialize] public BoardCellViewKey BoardCellViewPrefabKey { get; private set; }
        [OdinSerialize] public TileKey[] AvailableTileKeys { get; private set; } // TODO: This allows duplicates
        [OdinSerialize] public BoardVisualConfig BoardVisualConfig { get; private set; }
        [OdinSerialize] public TileMatchRuleConfig[] TileMatchRules { get; private set; }
        [OdinSerialize] public GameRuleConfig[] GameEndRules { get; private set; }
    }
}