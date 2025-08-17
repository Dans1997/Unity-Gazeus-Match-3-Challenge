using System.Collections.Generic;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;

namespace Gazeus.DesafioMatch3.Models
{
    public class DefaultBoardState : IBoardState
    {
        public List<List<TileInfo>> BoardTiles { get; set; }
        public int TileCount { get; set; }

        public DefaultBoardState(List<List<TileInfo>> tiles, int tileCount)
        {
            BoardTiles = tiles;
            TileCount = tileCount;
        }
    }
}