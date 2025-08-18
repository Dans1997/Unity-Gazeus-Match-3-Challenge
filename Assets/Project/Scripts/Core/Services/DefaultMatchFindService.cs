using System.Collections.Generic;
using System.Linq;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Services
{
    public class DefaultMatchFindService : IMatchFindService
    {
        public FindMatchResult FindMatches(IReadOnlyList<IReadOnlyList<TileInfo>> board, 
            IReadOnlyList<ITileMatchRule> matchRules)
        {
            var matches = new List<ITileMatchInfo>();
            var matchedPositionsSet = new HashSet<Vector2Int>();
            
            foreach (var rule in matchRules)
            {
                var ruleMatches = rule.FindMatches(board);
                if (ruleMatches is not { Count: > 0 }) continue;
                
                matches.AddRange(ruleMatches);

                foreach (var p in ruleMatches.SelectMany(ruleMatch => ruleMatch.Positions))
                {
                    matchedPositionsSet.Add(p);
                }
            }

            var newMatchResult = new FindMatchResult(matches, matchedPositionsSet);
            return newMatchResult;
        }
    }
}

// List<List<bool>> matchedTiles = new();
// for (var y = 0; y < board.Count; y++)
// {
//     matchedTiles.Add(new List<bool>(board[y].Count));
//     for (var x = 0; x < board[y].Count; x++)
//     {
//         matchedTiles[y].Add(false);
//     }
// }
//
// for (var y = 0; y < board.Count; y++)
// {
//     for (var x = 0; x < board[y].Count; x++)
//     {
//         if (x > 1 &&
//             board[y][x].Key == board[y][x - 1].Key &&
//             board[y][x - 1].Key == board[y][x - 2].Key)
//         {
//             matchedTiles[y][x] = true;
//             matchedTiles[y][x - 1] = true;
//             matchedTiles[y][x - 2] = true;
//         }
//
//         if (y > 1 &&
//             board[y][x].Key == board[y - 1][x].Key &&
//             board[y - 1][x].Key == board[y - 2][x].Key)
//         {
//             matchedTiles[y][x] = true;
//             matchedTiles[y - 1][x] = true;
//             matchedTiles[y - 2][x] = true;
//         }
//     }
// }