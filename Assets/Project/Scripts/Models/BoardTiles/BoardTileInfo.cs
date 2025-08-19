using System;
using Gazeus.DesafioMatch3.Project.Script.Enums;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct BoardTileInfo
    {
        public int Id { get; private set; }
        public TileKey Key { get; private set; }
        
        public BoardTileInfo(int id, TileKey key)
        {
            Id = id;
            Key = key;
        }
    }
}
