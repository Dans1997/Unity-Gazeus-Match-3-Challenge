using System.Collections.Generic;
using System.Linq;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Helpers;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Services
{
    public class DefaultTileSwapService : ITileSwapService
    {
        private readonly TileKey[] availableTileKeys;
        private readonly IMatchFindService matchFindService;
        private readonly ITileGenerationService tileGenerationService;
        private readonly ITileMatchRule[] tileMatchRules;
        private readonly IBoardEffectService boardEffectService;

        public DefaultTileSwapService(GameplayInfo gameplayInfo, ITileMatchRule[] tileMatchRules,
            IMatchFindService matchFindService, ITileGenerationService tileGenerationService,
            IBoardEffectService boardEffectService)
        {
            this.availableTileKeys = gameplayInfo.AvailableTileConfigs.Select(c => c.TileKey).ToArray();
            this.tileMatchRules = tileMatchRules;
            this.matchFindService = matchFindService;
            this.tileGenerationService = tileGenerationService;
            this.boardEffectService = boardEffectService;
        }

        public IBoardSwapResult SwapTile(IBoardState boardState, int fromX, int fromY, int toX, int toY)
        {
            var newBoard = GameplayHelpers.CopyGameplayBoard(boardState.BoardTiles);

            (newBoard[toY][toX], newBoard[fromY][fromX]) = (newBoard[fromY][fromX], newBoard[toY][toX]);

            List<BoardSequence> boardSequences = new();
            var findMatchResult = matchFindService.FindMatches(newBoard, tileMatchRules);
            var matchedPositionSet = findMatchResult.MatchedPositionsSet;

            while (matchedPositionSet.Count > 0)
            {
                var extra = boardEffectService.EvaluateEffects(boardState, findMatchResult);
                foreach (var e in extra) matchedPositionSet.Add(e);
                
                foreach (var matchedPosition in matchedPositionSet)
                {
                    newBoard[matchedPosition.y][matchedPosition.x] = new TileInfo { Id = -1, Key = (TileKey) (-1) };
                }

                Dictionary<int, MovedTileInfo> movedTiles = new();
                List<MovedTileInfo> movedTilesList = new();
                foreach (var matchedPosition in matchedPositionSet)
                {
                    var x = matchedPosition.x;
                    var y = matchedPosition.y;

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

                var newBoardSequence = new BoardSequence(movedTilesList, addedTiles, matchedPositionSet);
                boardSequences.Add(newBoardSequence);
                findMatchResult = matchFindService.FindMatches(newBoard, tileMatchRules);
                matchedPositionSet = findMatchResult.MatchedPositionsSet;
            }

            boardState.BoardTiles = newBoard;
            return new DefaultBoardSwapResult(boardState, boardSequences, 0);
        }
    }
}