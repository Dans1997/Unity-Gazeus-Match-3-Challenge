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
        public FindMatchResult FindMatches(IReadOnlyList<IReadOnlyList<BoardTileInfo>> board, 
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