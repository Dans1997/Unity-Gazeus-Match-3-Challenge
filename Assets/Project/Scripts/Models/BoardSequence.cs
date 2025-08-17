using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    public class BoardSequence
    {
        public List<MovedTileInfo> MovedTiles { get; set; }
        public List<AddedTileInfo> AddedTiles { get; set; }
        public List<Vector2Int> MatchedPosition { get; set; }
        
        public override string ToString()
        {
            var movedTilesStr = MovedTiles != null
                ? string.Join(", ", MovedTiles.Select(t => t.ToString()))
                : "null";

            var addedTilesStr = AddedTiles != null
                ? string.Join(", ", AddedTiles.Select(t => t.ToString()))
                : "null";

            var matchedPosStr = MatchedPosition != null
                ? string.Join(", ", MatchedPosition.Select(p => p.ToString()))
                : "null";

            return $"BoardSequence {{ \n" +
                   $"MovedTiles: [{movedTilesStr}], \n" +
                   $"AddedTiles: [{addedTilesStr}], \n" +
                   $"MatchedPosition: [{matchedPosStr}] \n" +
                   $"}}";
        }
    }
}
