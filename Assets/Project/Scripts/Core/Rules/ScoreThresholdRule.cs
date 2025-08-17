using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Scores;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Rules
{
    public class ScoreThresholdRule : IGameEndRule
    {
        public GameRuleKey GameRuleKey => GameRuleKey.ScoreThresholdRule;
        public string Message => $"Score Goal Reached!";
        public int Threshold { get; private set; }
        
        public ScoreThresholdRule(int threshold = 1000)
        {
            Threshold = threshold;
        }

        public bool IsGameOver(IGameplayService gameplayService, IScoreService scoreService)
        {
            return scoreService.CurrentScore >= Threshold;
        }
    }
}