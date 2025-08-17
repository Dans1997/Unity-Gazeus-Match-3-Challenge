using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface IMoveValidationService
    {
        bool IsValidMove(IBoardState boardState, int fromX, int fromY, int toX, int toY);
        bool HasAnyValidMove(IBoardState board);
        bool WouldSwapCreateMatch(IReadOnlyList<IReadOnlyList<TileInfo>> board, int x1, int y1, int x2, int y2);
        bool WouldFormMatchAt(IReadOnlyList<IReadOnlyList<TileInfo>> board, int x, int y, TileKey key);
    }
}