using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface ITileSwapService
    {
        public List<BoardSequence> SwapTile(ref List<List<TileInfo>> board, ref int tileCount, int fromX, int fromY, 
            int toX, int toY);
    }
}