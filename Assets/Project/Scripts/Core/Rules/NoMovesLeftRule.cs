using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Scores;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Rules
{
    public class NoMovesLeftRule : IGameEndRule
    {
        public GameRuleKey GameRuleKey => GameRuleKey.NoMovesLeftRule;
        public string Message => $"No More Moves Left!";
        
        public bool IsGameOver(IBoardService boardService, IScoreService scoreService)
        {
            return !boardService.HasAnyValidMove();
        }
    }
}