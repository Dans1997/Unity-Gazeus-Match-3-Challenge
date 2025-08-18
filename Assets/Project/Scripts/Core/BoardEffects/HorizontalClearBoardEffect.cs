using System.Collections.Generic;
using System.Linq;
using Gazeus.DesafioMatch3.Models.BoardEffects;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.BoardEffects;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.BoardEffects
{
    public class HorizontalClearBoardEffect : IBoardEffect
    {
        public BoardEffectConfig BoardEffectConfig { get; }
        public HorizontalClearBoardEffect(BoardEffectConfig config) => BoardEffectConfig = config;
        
        public IReadOnlyCollection<Vector2Int> Evaluate(ITileMatchInfo match, IBoardState boardState)
        { ;
            var y = match.Positions[0].y;
            var results = new HashSet<Vector2Int>();
            for (var x = 0; x < boardState.BoardWidth; x++)
            {
                results.Add(new Vector2Int(x, y));
            }
            return results;
        }
    }
}