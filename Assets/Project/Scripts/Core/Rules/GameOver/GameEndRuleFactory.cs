using System;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Rules
{
    public class GameEndRuleFactory
    {
        public IGameEndRule Create(GameRuleConfig config)
        {
            return config.RuleKey switch
            {
                GameRuleKey.NoMovesLeftRule => new NoMovesLeftRule(),
                GameRuleKey.ScoreThresholdRule => new ScoreThresholdRule(config.ScoreThreshold),
                GameRuleKey.TimerRule => new TimerRule(config.TimerSeconds),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}