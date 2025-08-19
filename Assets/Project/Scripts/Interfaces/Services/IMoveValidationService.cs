using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Models.BoardTiles;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface IMoveValidationService
    {
        bool IsValidMove(IBoardState boardState, int fromX, int fromY, int toX, int toY, out FindMatchResult findMatchResult);
        bool HasAnyValidMove(IBoardState boardState, out ValidTileMoveInfo firstValidMove);
    }
}