using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Core.Services
{
    public class GameplayService : IGameplayService
    {
        public List<List<Tile>> BoardTiles { get; private set; }
        public List<int> TilesTypes { get; private set; }
        public int TileCount { get; private set; }

        public bool IsValidMovement(int fromX, int fromY, int toX, int toY)
        {
            var newBoard = CopyBoard(BoardTiles);

            (newBoard[toY][toX], newBoard[fromY][fromX]) = (newBoard[fromY][fromX], newBoard[toY][toX]);

            for (var y = 0; y < newBoard.Count; y++)
            {
                for (var x = 0; x < newBoard[y].Count; x++)
                {
                    if (x > 1 &&
                        newBoard[y][x].Type == newBoard[y][x - 1].Type &&
                        newBoard[y][x - 1].Type == newBoard[y][x - 2].Type)
                    {
                        return true;
                    }

                    if (y > 1 &&
                        newBoard[y][x].Type == newBoard[y - 1][x].Type &&
                        newBoard[y - 1][x].Type == newBoard[y - 2][x].Type)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<List<Tile>> StartGame(int boardWidth, int boardHeight)
        {
            TilesTypes = new List<int> { 0, 1, 2, 3 };
            BoardTiles = CreateBoard(boardWidth, boardHeight, TilesTypes);

            return BoardTiles;
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
                        newBoard[y][x] = new Tile { Id = -1, Type = -1 };
                    }
                }

                Dictionary<int, MovedTileInfo> movedTiles = new();
                List<MovedTileInfo> movedTilesList = new();
                for (var i = 0; i < matchedPosition.Count; i++)
                {
                    var x = matchedPosition[i].x;
                    var y = matchedPosition[i].y;
                    if (y > 0)
                    {
                        for (var j = y; j > 0; j--)
                        {
                            var movedTile = newBoard[j - 1][x];
                            newBoard[j][x] = movedTile;
                            if (movedTile.Type <= -1) continue;
                            
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

                        newBoard[0][x] = new Tile { Id = -1, Type = -1 };
                    }
                }

                List<AddedTileInfo> addedTiles = new();
                for (var y = newBoard.Count - 1; y > -1; y--)
                {
                    for (var x = newBoard[y].Count - 1; x > -1; x--)
                    {
                        if (newBoard[y][x].Type != -1) continue;
                        
                        var tileType = Random.Range(0, TilesTypes.Count);
                        var tile = newBoard[y][x];
                        tile.Id = TileCount++;
                        tile.Type = TilesTypes[tileType];
                        addedTiles.Add(new AddedTileInfo
                        {
                            Position = new Vector2Int(x, y),
                            Type = tile.Type
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

        private static List<List<Tile>> CopyBoard(List<List<Tile>> boardToCopy)
        {
            List<List<Tile>> newBoard = new(boardToCopy.Count);
            for (var y = 0; y < boardToCopy.Count; y++)
            {
                newBoard.Add(new List<Tile>(boardToCopy[y].Count));
                for (var x = 0; x < boardToCopy[y].Count; x++)
                {
                    var tile = boardToCopy[y][x];
                    newBoard[y].Add(new Tile { Id = tile.Id, Type = tile.Type });
                }
            }

            return newBoard;
        }

        private List<List<Tile>> CreateBoard(int width, int height, List<int> tileTypes)
        {
            List<List<Tile>> board = new(height);
            TileCount = 0;

            for (var y = 0; y < height; y++)
            {
                board.Add(new List<Tile>(width));
                for (var x = 0; x < width; x++)
                {
                    board[y].Add(new Tile { Id = -1, Type = -1 });
                }
            }

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    List<int> noMatchTypes = new(tileTypes.Count);
                    for (var i = 0; i < tileTypes.Count; i++)
                    {
                        noMatchTypes.Add(TilesTypes[i]);
                    }

                    if (x > 1 && board[y][x - 1].Type == board[y][x - 2].Type)
                        noMatchTypes.Remove(board[y][x - 1].Type);

                    if (y > 1 && board[y - 1][x].Type == board[y - 2][x].Type)
                        noMatchTypes.Remove(board[y - 1][x].Type);

                    board[y][x].Id = TileCount++;
                    board[y][x].Type = noMatchTypes[Random.Range(0, noMatchTypes.Count)];
                }
            }

            return board;
        }

        private static List<List<bool>> FindMatches(List<List<Tile>> newBoard)
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
                        newBoard[y][x].Type == newBoard[y][x - 1].Type &&
                        newBoard[y][x - 1].Type == newBoard[y][x - 2].Type)
                    {
                        matchedTiles[y][x] = true;
                        matchedTiles[y][x - 1] = true;
                        matchedTiles[y][x - 2] = true;
                    }

                    if (y > 1 &&
                        newBoard[y][x].Type == newBoard[y - 1][x].Type &&
                        newBoard[y - 1][x].Type == newBoard[y - 2][x].Type)
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
            for (var y = 0; y < list.Count; y++)
            {
                for (var x = 0; x < list[y].Count; x++)
                {
                    if (list[y][x]) return true;
                }
            }

            return false;
        }
    }
}
