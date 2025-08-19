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

                var movedTilesList = DropTiles(newBoard);
                var addedTiles = GenerateNewTiles(boardState, newBoard);

                var newBoardSequence = new BoardSequence(movedTilesList, addedTiles, matchedPositions.ToList());
                newBoardSequences.Add(newBoardSequence);
            }
            while (findMatchResult.MatchedPositions.Count > 0);
            
            boardState.BoardTiles = newBoard;
            return new DefaultBoardSwapResult(boardState, newBoardSequences, 0);
        }
        
        private static List<MovedTileInfo> DropTiles(List<List<TileInfo>> newBoard)
        {
            var height = newBoard.Count;
            if (height == 0) return new List<MovedTileInfo>();
            var width = newBoard[0].Count;
            var movedTilesList = new List<MovedTileInfo>();

            for (var x = 0; x < width; x++)
            {
                var tilesInColumn = new List<TileInfo>();
                var originalPositions = new List<int>();
        
                for (var y = 0; y < height; y++)
                {
                    if (newBoard[y][x].Key == (TileKey)(-1)) continue;
                    tilesInColumn.Add(newBoard[y][x]);
                    originalPositions.Add(y);
                }
                
                for (var y = 0; y < height; y++)
                {
                    newBoard[y][x] = new TileInfo { Id = -1, Key = (TileKey)(-1) };
                }
                
                var startY = height - tilesInColumn.Count;
                for (var i = 0; i < tilesInColumn.Count; i++)
                {
                    var targetY = startY + i;
                    var tile = tilesInColumn[i];
                    var originalY = originalPositions[i];
            
                    newBoard[targetY][x] = tile;
                    
                    if (originalY != targetY)
                    {
                        movedTilesList.Add(new MovedTileInfo(
                            new Vector2Int(x, originalY),
                            new Vector2Int(x, targetY)
                        ));
                    }
                }
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
                        
                    newBoard[y][x] = tileGenerationService.GenerateNextTile(boardState);
                    addedTiles.Add(new AddedTileInfo(newBoard[y][x].Id, newBoard[y][x].Key, new Vector2Int(x, y)));
                }
            }

            return addedTiles;
        }
    }
}