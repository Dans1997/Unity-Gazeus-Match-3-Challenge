using System;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct TileMatchRuleConfig
    {
        [OdinSerialize] public TileMatchType TileMatchType { get; private set; }

        [ShowIf(nameof(IsLineMatch))]
        [OdinSerialize] public int MinLength { get; private set; }
        
        [ShowIf(nameof(TileMatchType), TileMatchType.SquareShapedMatch)]
        [OdinSerialize] public int SquareSize { get; private set; }
        
        private bool IsLineMatch => TileMatchType is TileMatchType.HorizontalLineMatch or TileMatchType.VerticalLineMatch;
    }
}