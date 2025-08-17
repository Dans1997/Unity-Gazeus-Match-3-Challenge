using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Services
{
    public interface IGameplayService
    {
        List<List<TileInfo>> BoardTiles { get; }
        int TileCount { get; }
        
        List<List<TileInfo>> CreateBoard();
        bool IsValidMovement(int fromX, int fromY, int toX, int toY);
        bool HasAnyValidMove();
        List<BoardSequence> SwapTile(int fromX, int fromY, int toX, int toY);
    }
}