using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Helpers
{
    public static class GameplayHelpers
    {
        public static List<List<TileInfo>> CopyGameplayBoard(List<List<TileInfo>> boardToCopy)
        {
            List<List<TileInfo>> newBoard = new(boardToCopy.Count);
            for (var y = 0; y < boardToCopy.Count; y++)
            {
                newBoard.Add(new List<TileInfo>(boardToCopy[y].Count));
                for (var x = 0; x < boardToCopy[y].Count; x++)
                {
                    var tile = boardToCopy[y][x];
                    newBoard[y].Add(new TileInfo { Id = tile.Id, Key = tile.Key });
                }
            }

            return newBoard;
        }
    }
}