using System.Collections.Generic;
using System.Linq;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Models.BoardEffects;
using Gazeus.Match3Challenge.Project.Scripts.Core.BoardEffects;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.BoardEffects;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Services
{
    public class DefaultBoardEffectService : IBoardEffectService
    {
        private readonly IBoardEffect[] boardEffects;

        public DefaultBoardEffectService(BoardEffectConfig[] boardEffectConfigs,
            BoardEffectFactory boardEffectFactory = null)
        {
            boardEffectFactory ??= new BoardEffectFactory();
            boardEffects = boardEffectConfigs?.Select(boardEffectFactory.Create).ToArray();
        }

        public HashSet<Vector2Int> EvaluateEffects(IBoardState boardState, FindMatchResult findMatchResult)
        {
            var results = new HashSet<Vector2Int>();
            foreach (var match in findMatchResult.Matches)
            {
                if (boardEffects == null) break;
                foreach (var effect in boardEffects)
                {
                    if (!IsMatchTrigger(effect, match)) continue;
                    var positions = effect.Evaluate(match, boardState);
                    if (positions == null) continue;
                    foreach (var p in positions) results.Add(p);
                }
            }
            return results;
        }

        private static bool IsMatchTrigger(IBoardEffect boardEffect, ITileMatchInfo match)
        {
            var effectConfig = boardEffect.BoardEffectConfig;
            var matchCount = match.Positions.Count;
            if (matchCount < effectConfig.MinValue) return false;
            
            if (effectConfig.HasTileMatchRuleTriggers && effectConfig.TileMatchRuleTriggers.Contains(match.TileMatchType))
            {
                return true;
            }

            if (effectConfig.HasDestroyedTilesTriggers && effectConfig.DestroyedTileTriggers.Contains(match.MatchedTileType))
            {
                return true;
            }
            
            return false;
        }
    }
}