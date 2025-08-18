using System.Collections.Generic;
using System.Linq;
using Gazeus.DesafioMatch3.Models.BoardEffects;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.BoardEffects;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.BoardEffects
{
    public class VerticalClearBoardEffect : IBoardEffect
    {
        public BoardEffectConfig BoardEffectConfig { get; }
        public VerticalClearBoardEffect(BoardEffectConfig config) => BoardEffectConfig = config;

        public IReadOnlyCollection<Vector2Int> Evaluate(ITileMatchInfo match, IBoardState boardState)
        {
            var board = boardState.BoardTiles;
            var height = board.Count;
            var x = match.Positions[0].x;
            var results = new HashSet<Vector2Int>();
            for (var y = 0; y < height; y++)
            {
                results.Add(new Vector2Int(x, y));
            }
            return results;
        }
    }
}