using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Scores;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules
{
    public interface IGameEndRule
    {
        GameRuleKey GameRuleKey { get; }
        string Message { get; }
        
        bool IsGameOver(IBoardService boardService, IScoreService scoreService);
    }
}