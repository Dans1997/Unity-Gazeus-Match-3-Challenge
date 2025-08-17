using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface IMoveValidationService
    {
        bool IsValidMove(List<List<TileInfo>> board, int fromX, int fromY, int toX, int toY);
        bool HasAnyValidMove(List<List<TileInfo>> board);
        public bool WouldSwapCreateMatch(List<List<TileInfo>> board, int x1, int y1, int x2, int y2);
        public bool WouldFormMatchAt(List<List<TileInfo>> board, int x, int y, TileKey key);
    }
}