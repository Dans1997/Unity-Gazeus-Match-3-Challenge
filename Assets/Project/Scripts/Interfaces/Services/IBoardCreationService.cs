using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface IBoardCreationService
    {
        public List<List<TileInfo>> CreateBoard(ref List<List<TileInfo>> board, ref int tileCount);
    }
}