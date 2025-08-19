using System;
using System.Collections.Generic;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public class DefaultBoardState : IBoardState
    {
        public List<List<BoardTileInfo>> BoardTiles { get; set; }
        public int TileCount { get; set; }
        public int BoardHeight => BoardTiles.Count;
        public int BoardWidth => BoardTiles[0].Count;

        public DefaultBoardState(List<List<BoardTileInfo>> tiles, int tileCount)
        {
            BoardTiles = tiles;
            TileCount = tileCount;
        }
    }
}