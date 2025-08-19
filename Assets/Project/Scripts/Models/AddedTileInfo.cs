using Gazeus.DesafioMatch3.Project.Script.Enums;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    public struct AddedTileInfo
    {
        public int Id { get; private set; }
        public TileKey Key { get; private set; }
        public Vector2Int Position { get; private set; }
        
        public AddedTileInfo(int id, TileKey key, Vector2Int position)
        {
            Id = id;
            Key = key;
            Position = position;
        }
        
        public override string ToString()
        {
            return $"{{ Position: {Position}, Tile: {Key.ToString()} }}";
        }
    }
}
