using Gazeus.DesafioMatch3.Project.Script.Enums;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    public struct AddedTileInfo
    {
        public TileKey Key { get; set; }
        public Vector2Int Position { get; set; }
        
        public override string ToString()
        {
            return $"{{ Position: {Position}, Tile: {Key.ToString()} }}";
        }
    }
}
