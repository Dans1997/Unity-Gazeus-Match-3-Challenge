using System;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public class TileMatchRuleConfig
    {
        [OdinSerialize] public TileMatchType TileMatchType { get; private set; }

        [ShowIf(nameof(IsLineMatch))]
        [OdinSerialize] public int MinLength { get; private set; } = 3;
        
        [ShowIf(nameof(TileMatchType), TileMatchType.SquareShapedMatch)]
        [OdinSerialize] public int SquareSize { get; private set; } = 2; 
        
        private bool IsLineMatch => TileMatchType is TileMatchType.HorizontalLineMatch or TileMatchType.VerticalLineMatch;
    }
}