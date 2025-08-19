using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Helpers
{
    public static class GameplayHelpers
    {
        public static List<List<BoardTileInfo>> CopyGameplayBoard(IReadOnlyList<IReadOnlyList<BoardTileInfo>> boardToCopy)
        {
            List<List<BoardTileInfo>> newBoard = new(boardToCopy.Count);
            for (var y = 0; y < boardToCopy.Count; y++)
            {
                newBoard.Add(new List<BoardTileInfo>(boardToCopy[y].Count));
                for (var x = 0; x < boardToCopy[y].Count; x++)
                {
                    var tile = boardToCopy[y][x];
                    newBoard[y].Add(new BoardTileInfo(tile.Id, tile.Key));
                }
            }

            return newBoard;
        }
    }
}