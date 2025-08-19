using System;
using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Scripts.Helpers;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;
using Gazeus.Match3Challenge.Project.Scripts.Models.BoardTiles;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Services
{
    public class DefaultMoveValidationService : IMoveValidationService
    {
        private readonly IMatchFindService matchFindService;
        private readonly IReadOnlyList<ITileMatchRule> matchRules;

        public DefaultMoveValidationService(IMatchFindService matchFindService, IReadOnlyList<ITileMatchRule> matchRules)
        {
            this.matchFindService = matchFindService;
            this.matchRules = matchRules ?? Array.Empty<ITileMatchRule>();
        }
        
        public bool IsValidMove(IBoardState boardState, int fromX, int fromY, int toX, int toY, 
            out FindMatchResult findMatchResult)
        {
            var board = GameplayHelpers.CopyGameplayBoard(boardState.BoardTiles);
            (board[toY][toX], board[fromY][fromX]) = (board[fromY][fromX], board[toY][toX]);
            findMatchResult = matchFindService.FindMatches(board, matchRules);
            return findMatchResult.Matches.Count > 0;
        }
        
        public bool HasAnyValidMove(IBoardState boardState, out ValidTileMoveInfo firstValidMove)
        {
            firstValidMove = null;
            
            for (var y = 0; y < boardState.BoardHeight; y++)
            {
                for (var x = 0; x < boardState.BoardWidth; x++)
                {
                    if (x + 1 < boardState.BoardWidth && IsValidMove(boardState, x, y, x + 1, y, out var findMatchResult1))
                    {
                        firstValidMove = new ValidTileMoveInfo(new Vector2Int(x, y), new Vector2Int(x + 1, y), findMatchResult1);
                        return true;
                    }

                    if (y + 1 < boardState.BoardHeight  && IsValidMove(boardState, x, y, x, y + 1, out var findMatchResult))
                    {
                        firstValidMove = new ValidTileMoveInfo(new Vector2Int(x, y), new Vector2Int(x, y + 1), findMatchResult);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}