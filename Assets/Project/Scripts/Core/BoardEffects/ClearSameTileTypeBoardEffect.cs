using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models.BoardEffects;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.BoardEffects;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.BoardEffects
{
    public class ClearSameTileTypeBoardEffect : IBoardEffect
    {
        public BoardEffectConfig BoardEffectConfig { get; }
        public ClearSameTileTypeBoardEffect(BoardEffectConfig config) => BoardEffectConfig = config;
        
        public IReadOnlyCollection<Vector2Int> Evaluate(ITileMatchInfo match, IBoardState boardState)
        {
            var results = new HashSet<Vector2Int>();
            var key = match.MatchedTileType;
            for (var y = 0; y < boardState.BoardHeight; y++)
            {
                for (var x = 0; x < boardState.BoardWidth; x++)
                {
                    if (boardState.BoardTiles[y][x].Key == key) results.Add(new Vector2Int(x, y));
                }
            }
            return results;
        }
    }
}