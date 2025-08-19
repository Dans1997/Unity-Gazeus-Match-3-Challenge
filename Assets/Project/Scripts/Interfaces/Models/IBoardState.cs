using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models
{
    public interface IBoardState
    {
        List<List<BoardTileInfo>> BoardTiles { get; set; }
        public int TileCount { get; set; }
        int BoardHeight { get; }
        public int BoardWidth { get; }
    }
}