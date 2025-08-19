using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public class BoardSequence
    {
        public IReadOnlyList<MovedTileInfo> MovedTiles { get; private set; }
        public IReadOnlyList<AddedTileInfo> AddedTiles { get; private set; }
        public IReadOnlyList<Vector2Int> MatchedPositions { get; private set; }
        
        public BoardSequence(IReadOnlyList<MovedTileInfo> movedTiles, IReadOnlyList<AddedTileInfo> addedTiles, 
            IReadOnlyList<Vector2Int> matchedPositions)
        {
            MovedTiles = movedTiles;
            AddedTiles = addedTiles;
            MatchedPositions = matchedPositions;
        }
        
        public override string ToString()
        {
            var movedTilesStr = MovedTiles != null
                ? string.Join(", ", MovedTiles.Select(t => t.ToString()))
                : "null";

            var addedTilesStr = AddedTiles != null
                ? string.Join(", ", AddedTiles.Select(t => t.ToString()))
                : "null";

            var matchedPosStr = MatchedPositions != null
                ? string.Join(", ", MatchedPositions.Select(p => p.ToString()))
                : "null";

            return $"BoardSequence {{ \n" +
                   $"MovedTiles: [{movedTilesStr}], \n" +
                   $"AddedTiles: [{addedTilesStr}], \n" +
                   $"MatchedPosition: [{matchedPosStr}] \n" +
                   $"}}";
        }
    }
}
