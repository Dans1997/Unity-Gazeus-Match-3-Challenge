using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Services
{
    public interface IGameplayService
    {
        List<List<Tile>> BoardTiles { get; }
        List<int> TilesTypes { get; }
        int TileCount { get; }
        
        bool IsValidMovement(int fromX, int fromY, int toX, int toY);
        List<List<Tile>> StartGame(int boardWidth, int boardHeight);
        List<BoardSequence> SwapTile(int fromX, int fromY, int toX, int toY);
    }
}