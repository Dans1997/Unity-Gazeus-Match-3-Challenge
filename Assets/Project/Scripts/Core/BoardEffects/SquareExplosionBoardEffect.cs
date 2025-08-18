using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models.BoardEffects;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.BoardEffects;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.BoardEffects
{
    public class SquareExplosionBoardEffect : IBoardEffect
    {
        public BoardEffectConfig BoardEffectConfig { get; }
        public SquareExplosionBoardEffect(BoardEffectConfig config) => BoardEffectConfig = config;

        public IReadOnlyCollection<Vector2Int> Evaluate(ITileMatchInfo match, IBoardState boardState)
        {
            var results = new HashSet<Vector2Int>();
            var center = match.Positions[0];
            var r = Mathf.Max(0, BoardEffectConfig.ExplosionRadius);
            var minX = Mathf.Max(0, center.x - r);
            var maxX = Mathf.Min(boardState.BoardWidth - 1, center.x + r);
            var minY = Mathf.Max(0, center.y - r);
            var maxY = Mathf.Min(boardState.BoardHeight - 1, center.y + r);

            for (var yy = minY; yy <= maxY; yy++)
            {
                for (var xx = minX; xx <= maxX; xx++)
                {
                    results.Add(new Vector2Int(xx, yy));
                }
            }

            return results;
        }
    }
}