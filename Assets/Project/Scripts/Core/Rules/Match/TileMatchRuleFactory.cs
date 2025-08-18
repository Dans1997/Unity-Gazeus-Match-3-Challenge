using System;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Rules.Match
{
    public class TileMatchRuleFactory
    {
        public ITileMatchRule Create(TileMatchRuleConfig config)
        {
            return config.TileMatchType switch
            {
                TileMatchType.HorizontalLineMatch => new HorizontalLineTileMatchRule(config.MinLength),
                TileMatchType.VerticalLineMatch => new VerticalLineTileMatchRule(config.MinLength),
                TileMatchType.SquareShapedMatch => new SquareShapedTileMatchRule(config.SquareSize),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}