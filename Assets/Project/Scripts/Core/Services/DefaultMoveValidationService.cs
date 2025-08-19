using System;
using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Scripts.Helpers;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Services
{
    public class DefaultMoveValidationService : IMoveValidationService
    {
        private readonly IMatchFindService matchFindService;
        private readonly IReadOnlyList<ITileMatchRule> matchRules;
        private readonly int localPadding;

        public DefaultMoveValidationService(IMatchFindService matchFindService, IReadOnlyList<ITileMatchRule> matchRules, 
            int localPadding = 2)
        {
            this.matchFindService = matchFindService;
            this.matchRules = matchRules ?? Array.Empty<ITileMatchRule>();
            this.localPadding = Math.Max(1, localPadding);
        }
        
        public bool IsValidMove(IBoardState boardState, int fromX, int fromY, int toX, int toY)
        {
            if (boardState == null) return false;
            var board = GameplayHelpers.CopyGameplayBoard(boardState.BoardTiles);
            (board[toY][toX], board[fromY][fromX]) = (board[fromY][fromX], board[toY][toX]);
            var result = matchFindService.FindMatches(board, matchRules);
            return result.Matches.Count > 0;
        }
        
        public bool HasAnyValidMove(IBoardState boardState)
        {
            for (var y = 0; y < boardState.BoardHeight; y++)
            {
                for (var x = 0; x < boardState.BoardWidth; x++)
                {
                    if (x + 1 < boardState.BoardWidth && WouldSwapCreateMatch(boardState, x, y, x + 1, y))
                        return true;
                    
                    if (y + 1 < boardState.BoardWidth && WouldSwapCreateMatch(boardState, x, y, x, y + 1))
                        return true;
                }
            }

            return false;
        }

        private bool WouldSwapCreateMatch(IBoardState boardState, int x1, int y1, int x2, int y2)
        {
            var board = boardState.BoardTiles;

            var minX = Math.Max(0, Math.Min(x1, x2) - localPadding);
            var minY = Math.Max(0, Math.Min(y1, y2) - localPadding);
            var maxX = Math.Min(boardState.BoardWidth - 1, Math.Max(x1, x2) + localPadding);
            var maxY = Math.Min(boardState.BoardHeight - 1, Math.Max(y1, y2) + localPadding);

            var sub = new List<List<BoardTileInfo>>(maxY - minY + 1);
            for (var sy = minY; sy <= maxY; sy++)
            {
                var row = new List<BoardTileInfo>(maxX - minX + 1);
                for (var sx = minX; sx <= maxX; sx++)
                {
                    row.Add(new BoardTileInfo(board[sy][sx].Id, board[sy][sx].Key));
                }
                sub.Add(row);
            }

            var localFromX = x1 - minX;
            var localFromY = y1 - minY;
            var localToX = x2 - minX;
            var localToY = y2 - minY;

            (sub[localToY][localToX], sub[localFromY][localFromX]) = (sub[localFromY][localFromX], sub[localToY][localToX]);

            var result = matchFindService.FindMatches(sub, matchRules);
            return result.Matches.Count > 0;
        }
    }
}