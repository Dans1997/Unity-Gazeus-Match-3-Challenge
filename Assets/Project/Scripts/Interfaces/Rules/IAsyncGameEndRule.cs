using System;
using Cysharp.Threading.Tasks;

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
        
        UniTask StartAsync(Action<IGameEndRule> onGameEnd);
    }
}