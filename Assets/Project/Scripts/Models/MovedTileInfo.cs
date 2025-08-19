using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    public class MovedTileInfo
    {
        public Vector2Int From { get; }
        public Vector2Int To { get; set; }
        
        public MovedTileInfo(Vector2Int from, Vector2Int to)
        {
            From = from;
            To = to;
        }
        
        public override string ToString()
        {
            return $"{{ From: {From}, To: {To} }}";
        }
    }
}
