using System;
using Cysharp.Threading.Tasks;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Scores;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules
{
    public interface IAsyncGameEndRule : IGameEndRule
    {
        event Action<float> TimeLeftUpdated;
        
        float ElapsedTime { get; }
        float TimeLeft { get; }
        float StartTime { get; }
        float EndTime { get; }
        public float UpdateIntervalInSeconds { get; } 
        
        UniTask StartAsync(IBoardService boardService, IScoreService scoreService, Action<IGameEndRule> onGameEnd);
    }
}