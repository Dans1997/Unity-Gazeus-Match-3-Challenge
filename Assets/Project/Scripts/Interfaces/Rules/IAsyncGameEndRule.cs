using System;
using Cysharp.Threading.Tasks;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Scores;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules
{
    public interface IAsyncGameEndRule : IGameEndRule
    {
        float ElapsedTime { get; }
        float TimeLeft { get; }
        float StartTime { get; }
        float EndTime { get; }
        
        UniTask StartAsync(IGameplayService gameplayService, IScoreService scoreService, Action<IGameEndRule> onGameEnd);
    }
}