using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface IMoveValidationService
    {
        bool IsValidMove(IBoardState boardState, int fromX, int fromY, int toX, int toY);
        bool HasAnyValidMove(IBoardState board);
    }
}