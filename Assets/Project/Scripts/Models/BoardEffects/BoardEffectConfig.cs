using System;
using System.Collections.Generic;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Gazeus.DesafioMatch3.Models.BoardEffects
{
    [Serializable]
    public class BoardEffectConfig
    {
        [OdinSerialize] public BoardEffectType BoardEffectType { get; private set; }
        [ShowIf(nameof(BoardEffectType), BoardEffectType.SquareExplosionBoardEffect)]
        [OdinSerialize, PropertyRange(1, 100)] public int ExplosionRadius { get; private set; } = 3;

        [OdinSerialize] public HashSet<TileMatchType> TileMatchRuleTriggers { get; private set; }
        [OdinSerialize] public HashSet<TileKey> DestroyedTileTriggers { get; private set; }
        
        [ShowIf(nameof(HasTileTriggers))]
        [OdinSerialize, PropertyRange(1, 100)] public int MinValue { get; private set; } = 3;

        public bool HasTileTriggers => HasTileMatchRuleTriggers || HasDestroyedTilesTriggers;
        public bool HasTileMatchRuleTriggers => TileMatchRuleTriggers is { Count: > 0 };
        public bool HasDestroyedTilesTriggers => DestroyedTileTriggers is { Count: > 0 };
    }
}