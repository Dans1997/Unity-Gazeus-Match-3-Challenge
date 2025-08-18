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
                TileMatchType.StraightLine => new StraightLineTileMatchRule(config.MinLength),
                TileMatchType.SquareShaped => new SquareShapedTileMatchRule(config.SquareSize),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}