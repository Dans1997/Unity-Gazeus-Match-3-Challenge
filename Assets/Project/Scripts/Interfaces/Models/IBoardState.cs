using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models
{
    public interface IBoardState
    {
        List<List<TileInfo>> BoardTiles { get; set; }
        int BoardHeight { get; }
        public int BoardWidth { get; }
        public int TileCount { get; set; }
    }
}