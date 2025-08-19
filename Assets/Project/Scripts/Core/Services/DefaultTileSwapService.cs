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
        private readonly IMatchFindService matchFindService;
        private readonly ITileGenerationService tileGenerationService;
        private readonly ITileMatchRule[] tileMatchRules;
        private readonly IBoardEffectService boardEffectService;

        public DefaultTileSwapService(ITileMatchRule[] tileMatchRules,
            IMatchFindService matchFindService, ITileGenerationService tileGenerationService,
            IBoardEffectService boardEffectService)
        {
            this.tileMatchRules = tileMatchRules;
            this.matchFindService = matchFindService;
            this.tileGenerationService = tileGenerationService;
            this.boardEffectService = boardEffectService;
        }

        public IBoardSwapResult SwapTile(IBoardState boardState, int fromX, int fromY, int toX, int toY)
        {
            var newBoardSequences = new List<BoardSequence>();
            var newBoard = GameplayHelpers.CopyGameplayBoard(boardState.BoardTiles);
            FindMatchResult findMatchResult;
        
            (newBoard[toY][toX], newBoard[fromY][fromX]) = (newBoard[fromY][fromX], newBoard[toY][toX]);
            
            do
            {
                findMatchResult = matchFindService.FindMatches(newBoard, tileMatchRules);
                var matchedPositions = findMatchResult.MatchedPositions;
                if (matchedPositions.Count <= 0) break;
                
                matchedPositions.UnionWith(boardEffectService.EvaluateEffects(boardState, findMatchResult));
                
                foreach (var matchedPosition in matchedPositions)
                {
                    newBoard[matchedPosition.y][matchedPosition.x] = new TileInfo { Id = -1, Key = (TileKey) (-1) };
                }

                var movedTilesList = MoveTilesDown(matchedPositions, newBoard);
                var addedTiles = GenerateNewTiles(boardState, newBoard);

                var newBoardSequence = new BoardSequence(movedTilesList, addedTiles, matchedPositions.ToList());
                newBoardSequences.Add(newBoardSequence);
            }
            while (findMatchResult.MatchedPositions.Count > 0);
            
            boardState.BoardTiles = newBoard;
            return new DefaultBoardSwapResult(boardState, newBoardSequences, 0);
        }

        private static List<MovedTileInfo> MoveTilesDown(IReadOnlyCollection<Vector2Int> matchedPositions, 
            List<List<TileInfo>> newBoard)
        {
            Dictionary<int, MovedTileInfo> movedTiles = new();
            List<MovedTileInfo> movedTilesList = new();
            foreach (var matchedPosition in matchedPositions)
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

            return movedTilesList;
        }
        
        private List<AddedTileInfo> GenerateNewTiles(IBoardState boardState, List<List<TileInfo>> newBoard)
        {
            List<AddedTileInfo> addedTiles = new();
            for (var y = newBoard.Count - 1; y > -1; y--)
            {
                for (var x = newBoard[y].Count - 1; x > -1; x--)
                {
                    if (newBoard[y][x].Key != (TileKey) (-1)) continue;
                        
                    var tile = newBoard[y][x];
                    tileGenerationService.GenerateNextTile(newBoard[y][x], boardState);

                    addedTiles.Add(new AddedTileInfo(tile.Id, tile.Key, new Vector2Int(x, y)));
                }
            }

            return addedTiles;
        }
    }
}