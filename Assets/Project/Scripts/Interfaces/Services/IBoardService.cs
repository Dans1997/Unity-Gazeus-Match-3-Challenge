using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Models.BoardTiles;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Services
{
    public interface IBoardService
    {
        IBoardState BoardState { get; }
        void CreateBoard();
        bool IsValidMove(int fromX, int fromY, int toX, int toY);
        bool HasAnyValidMove(out ValidTileMoveInfo firstValidMove);
        IBoardSwapResult SwapTile(int fromX, int fromY, int toX, int toY);
    }
}