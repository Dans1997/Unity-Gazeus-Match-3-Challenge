using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Services
{
    public interface IBoardService
    {
        IBoardState BoardState { get; }
        void CreateBoard();
        bool IsValidMovement(int fromX, int fromY, int toX, int toY);
        bool HasAnyValidMove();
        IBoardSwapResult SwapTile(int fromX, int fromY, int toX, int toY);
    }
}