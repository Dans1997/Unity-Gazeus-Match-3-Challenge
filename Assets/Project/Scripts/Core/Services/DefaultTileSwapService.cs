using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Helpers;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Services
{
    public class DefaultTileSwapService : ITileSwapService
    {
        private readonly IMatchFindService matchFindService;
        private readonly ITileGenerationService tileGenerationService;
        private readonly TileKey[] availableTileKeys;

        public DefaultTileSwapService(IMatchFindService matchFindService, ITileGenerationService tileGenerationService,
            TileKey[] availableTileKeys)
        {
            this.matchFindService = matchFindService ?? new DefaultMatchFindService();
            this.tileGenerationService = tileGenerationService;
            this.availableTileKeys = availableTileKeys;
        }
        
        public IBoardSwapResult SwapTile(IBoardState boardState, int fromX, int fromY, int toX, int toY)
        {
            var newBoard = GameplayHelpers.CopyGameplayBoard(boardState.BoardTiles);

            (newBoard[toY][toX], newBoard[fromY][fromX]) = (newBoard[fromY][fromX], newBoard[toY][toX]);

            List<BoardSequence> boardSequences = new();
            var matchedTiles = matchFindService.FindMatches(newBoard);

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
                        
                        var tile = newBoard[y][x];
                        tileGenerationService.GenerateNextTile(newBoard[y][x], availableTileKeys, boardState);

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
                matchedTiles = matchFindService.FindMatches(newBoard);
            }

            boardState.BoardTiles = newBoard;
            return new DefaultBoardSwapResult(boardState, boardSequences, 0);
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
    }
}