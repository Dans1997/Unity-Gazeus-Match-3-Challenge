using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gazeus.DesafioMatch3.Core.Services
{
    public class GameplayService : IGameplayService
    {
        public GameplayInfo GameplayInfo { get; private set; }
        public TileKey[] AvailableTileKeys { get; private set; }
        public List<List<TileInfo>> BoardTiles { get; private set; }
        public int TileCount { get; private set; }

        public GameplayService(GameplayInfo gameplayInfo)
        {
            GameplayInfo = gameplayInfo;
            AvailableTileKeys = gameplayInfo.AvailableTileKeys;
        }

        public List<List<TileInfo>> CreateBoard()
        {
            BoardTiles = CreateBoard(GameplayInfo.BoardWidth, GameplayInfo.BoardHeight, AvailableTileKeys);
            return BoardTiles;
        }

        public bool IsValidMovement(int fromX, int fromY, int toX, int toY)
        {
            var newBoard = CopyBoard(BoardTiles);

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

        public bool HasAnyValidMove()
        {
            if (BoardTiles == null || BoardTiles.Count == 0) return false;
            var height = BoardTiles.Count;
            var width = BoardTiles[0].Count;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (x + 1 < width && WouldSwapCreateMatch(x, y, x + 1, y))
                        return true;
                    
                    if (y + 1 < height && WouldSwapCreateMatch(x, y, x, y + 1))
                        return true;
                }
            }

            return false;
        }

        private bool WouldSwapCreateMatch(int x1, int y1, int x2, int y2)
        {
            var k1 = BoardTiles[y1][x1].Key;
            var k2 = BoardTiles[y2][x2].Key;
            if (k1 == k2) return false;
            
            if (WouldFormMatchAt(x1, y1, k2)) return true;
            if (WouldFormMatchAt(x2, y2, k1)) return true;
            return false;
        }
        
        private bool WouldFormMatchAt(int x, int y, TileKey key)
        {
            if (key == (TileKey)(-1)) return false;

            var width = BoardTiles[y].Count;
            var height = BoardTiles.Count;
            var count = 1;
            
            for (var ix = x - 1; ix >= 0; ix--)
            {
                if (BoardTiles[y][ix].Key == key) count++;
                else break;
            }
            
            for (var ix = x + 1; ix < width; ix++)
            {
                if (BoardTiles[y][ix].Key == key) count++;
                else break;
            }
            if (count >= 3) return true;
            
            count = 1;

            for (var iy = y - 1; iy >= 0; iy--)
            {
                if (BoardTiles[iy][x].Key == key) count++;
                else break;
            }

            for (var iy = y + 1; iy < height; iy++)
            {
                if (BoardTiles[iy][x].Key == key) count++;
                else break;
            }
            
            return count >= 3;
        }

        public List<BoardSequence> SwapTile(int fromX, int fromY, int toX, int toY)
        {
            var newBoard = CopyBoard(BoardTiles);

            (newBoard[toY][toX], newBoard[fromY][fromX]) = (newBoard[fromY][fromX], newBoard[toY][toX]);

            List<BoardSequence> boardSequences = new();
            var matchedTiles = FindMatches(newBoard);

            while (HasMatch(matchedTiles))
            {
                List<Vector2Int> matchedPosition = new();
                for (var y = 0; y < newBoard.Count; y++)
                {
                    for (var x = 0; x < newBoard[y].Count; x++)
                    {
                        if (!matchedTiles[y][x]) continue;
                        matchedPosition.Add(new Vector2Int(x, y));
                        newBoard[y][x] = new TileInfo { Id = -1, Key = (TileKey) (-1) };
                    }
                }

                Dictionary<int, MovedTileInfo> movedTiles = new();
                List<MovedTileInfo> movedTilesList = new();
                for (var i = 0; i < matchedPosition.Count; i++)
                {
                    var x = matchedPosition[i].x;
                    var y = matchedPosition[i].y;
                    if (y <= 0) continue;
                    
                    for (var j = y; j > 0; j--)
                    {
                        var movedTile = newBoard[j - 1][x];
                        newBoard[j][x] = movedTile;
                        if (movedTile.Key == (TileKey) (-1)) continue;
                            
                        if (movedTiles.TryGetValue(movedTile.Id, out var tile))
                        {
                            tile.To = new Vector2Int(x, j);
                        }
                        else
                        {
                            MovedTileInfo movedTileInfo = new()
                            {
                                From = new Vector2Int(x, j - 1),
                                To = new Vector2Int(x, j)
                            };
                            movedTiles.Add(movedTile.Id, movedTileInfo);
                            movedTilesList.Add(movedTileInfo);
                        }
                    }

                    newBoard[0][x] = new TileInfo { Id = -1, Key = (TileKey) (-1) };
                }

                List<AddedTileInfo> addedTiles = new();
                for (var y = newBoard.Count - 1; y > -1; y--)
                {
                    for (var x = newBoard[y].Count - 1; x > -1; x--)
                    {
                        if (newBoard[y][x].Key != (TileKey) (-1)) continue;
                        
                        var tileType = Random.Range(0, AvailableTileKeys.Length);
                        var tile = newBoard[y][x];
                        tile.Id = TileCount++;
                        tile.Key = AvailableTileKeys[tileType];
                        addedTiles.Add(new AddedTileInfo
                        {
                            Position = new Vector2Int(x, y),
                            Key = tile.Key
                        });
                    }
                }

                BoardSequence sequence = new()
                {
                    MatchedPosition = matchedPosition,
                    MovedTiles = movedTilesList,
                    AddedTiles = addedTiles
                };
                boardSequences.Add(sequence);
                matchedTiles = FindMatches(newBoard);
            }

            BoardTiles = newBoard;

            return boardSequences;
        }

        private List<List<TileInfo>> CreateBoard(int width, int height, TileKey[] tileTypes)
        {
            List<List<TileInfo>> board = new(height);
            TileCount = 0;

            for (var y = 0; y < height; y++)
            {
                board.Add(new List<TileInfo>(width));
                for (var x = 0; x < width; x++)
                {
                    board[y].Add(new TileInfo { Id = -1, Key = (TileKey) (-1) });
                }
            }

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    List<TileKey> noMatchTypes = new(tileTypes.Length);
                    for (var i = 0; i < tileTypes.Length; i++)
                    {
                        noMatchTypes.Add(AvailableTileKeys[i]);
                    }

                    if (x > 1 && board[y][x - 1].Key == board[y][x - 2].Key)
                        noMatchTypes.Remove(board[y][x - 1].Key);

                    if (y > 1 && board[y - 1][x].Key == board[y - 2][x].Key)
                        noMatchTypes.Remove(board[y - 1][x].Key);

                    board[y][x].Id = TileCount++;
                    board[y][x].Key = noMatchTypes[Random.Range(0, noMatchTypes.Count)];
                }
            }

            return board;
        }

        private static List<List<bool>> FindMatches(List<List<TileInfo>> newBoard)
        {
            List<List<bool>> matchedTiles = new();
            for (var y = 0; y < newBoard.Count; y++)
            {
                matchedTiles.Add(new List<bool>(newBoard[y].Count));
                for (var x = 0; x < newBoard[y].Count; x++)
                {
                    matchedTiles[y].Add(false);
                }
            }

            for (var y = 0; y < newBoard.Count; y++)
            {
                for (var x = 0; x < newBoard[y].Count; x++)
                {
                    if (x > 1 &&
                        newBoard[y][x].Key == newBoard[y][x - 1].Key &&
                        newBoard[y][x - 1].Key == newBoard[y][x - 2].Key)
                    {
                        matchedTiles[y][x] = true;
                        matchedTiles[y][x - 1] = true;
                        matchedTiles[y][x - 2] = true;
                    }

                    if (y > 1 &&
                        newBoard[y][x].Key == newBoard[y - 1][x].Key &&
                        newBoard[y - 1][x].Key == newBoard[y - 2][x].Key)
                    {
                        matchedTiles[y][x] = true;
                        matchedTiles[y - 1][x] = true;
                        matchedTiles[y - 2][x] = true;
                    }
                }
            }

            return matchedTiles;
        }

        private static bool HasMatch(List<List<bool>> list)
        {
            foreach (var t in list)
            {
                foreach (var t1 in t)
                {
                    if (t1) return true;
                }
            }

            return false;
        }
        
        private static List<List<TileInfo>> CopyBoard(List<List<TileInfo>> boardToCopy)
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
