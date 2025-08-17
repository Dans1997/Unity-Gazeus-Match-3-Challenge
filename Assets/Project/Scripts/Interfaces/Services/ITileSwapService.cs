using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface ITileSwapService
    {
        IBoardSwapResult SwapTile(IBoardState boardState, int fromX, int fromY, int toX, int toY);
    }
}