using System;
using Gazeus.DesafioMatch3.Models.BoardEffects;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.BoardEffects;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.BoardEffects
{
    public class BoardEffectFactory
    {
        public IBoardEffect Create(BoardEffectConfig config)
        {
            return config.BoardEffectType switch
            {
                BoardEffectType.HorizontalClearEffect => new HorizontalClearBoardEffect(config),
                BoardEffectType.VerticalClearEffect => new VerticalClearBoardEffect(config),
                BoardEffectType.ClearSameTileTypeEffect => new ClearSameTileTypeBoardEffect(config),
                BoardEffectType.SquareExplosionBoardEffect => new SquareExplosionBoardEffect(config),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}