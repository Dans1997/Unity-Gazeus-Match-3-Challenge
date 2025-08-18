using System.Collections.Generic;
using System.Linq;
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
            var board = boardState.BoardTiles;
            var height = board.Count;
            var width = board[0].Count;
            var key = match.MatchedTileType;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (board[y][x].Key == key) results.Add(new Vector2Int(x, y));
                }
            }
            return results;
        }
    }
}