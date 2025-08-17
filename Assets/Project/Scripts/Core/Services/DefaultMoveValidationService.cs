using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Helpers;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Services
{
    public class DefaultMoveValidationService : IMoveValidationService
    {
        public bool IsValidMove(IBoardState boardState, int fromX, int fromY, int toX, int toY)
        {
            var newBoard = GameplayHelpers.CopyGameplayBoard(boardState.BoardTiles);

            (newBoard[toY][toX], newBoard[fromY][fromX]) = (newBoard[fromY][fromX], newBoard[toY][toX]);

            for (var y = 0; y < newBoard.Count; y++)
            {
                for (var x = 0; x < newBoard[y].Count; x++)
                {
                    if (x > 1 &&
                        newBoard[y][x].Key == newBoard[y][x - 1].Key &&
                        newBoard[y][x - 1].Key == newBoard[y][x - 2].Key)
                    {
                        return true;
                    }

                    if (y > 1 &&
                        newBoard[y][x].Key == newBoard[y - 1][x].Key &&
                        newBoard[y - 1][x].Key == newBoard[y - 2][x].Key)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public bool HasAnyValidMove(IBoardState boardState)
        {
            if (boardState == null || boardState.TileCount == 0) return false;
            var height = boardState.TileCount;
            var width = boardState.BoardTiles[0].Count;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (x + 1 < width && WouldSwapCreateMatch(boardState.BoardTiles, x, y, x + 1, y))
                        return true;
                    
                    if (y + 1 < height && WouldSwapCreateMatch(boardState.BoardTiles, x, y, x, y + 1))
                        return true;
                }
            }

            return false;
        }

        public bool WouldSwapCreateMatch(IReadOnlyList<IReadOnlyList<TileInfo>> board, int x1, int y1, int x2, int y2)
        {
            var k1 = board[y1][x1].Key;
            var k2 = board[y2][x2].Key;
            if (k1 == k2) return false;
            
            if (WouldFormMatchAt(board, x1, y1, k2)) return true;
            if (WouldFormMatchAt(board, x2, y2, k1)) return true;
            return false;
        }
        
        public bool WouldFormMatchAt(IReadOnlyList<IReadOnlyList<TileInfo>> board, int x, int y, TileKey key)
        {
            if (key == (TileKey)(-1)) return false;

            var width = board[y].Count;
            var height = board.Count;
            var count = 1;
            
            for (var ix = x - 1; ix >= 0; ix--)
            {
                if (board[y][ix].Key == key) count++;
                else break;
            }
            
            for (var ix = x + 1; ix < width; ix++)
            {
                if (board[y][ix].Key == key) count++;
                else break;
            }
            if (count >= 3) return true;
            
            count = 1;

            for (var iy = y - 1; iy >= 0; iy--)
            {
                if (board[iy][x].Key == key) count++;
                else break;
            }

            for (var iy = y + 1; iy < height; iy++)
            {
                if (board[iy][x].Key == key) count++;
                else break;
            }
            
            return count >= 3;
        }
    }
}