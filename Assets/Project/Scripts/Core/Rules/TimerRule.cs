using System;
using Cysharp.Threading.Tasks;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Scores;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Rules
{
    public class TimerRule : IAsyncGameEndRule
    {
        public GameRuleKey GameRuleKey => GameRuleKey.TimerRule;
        public string Message => $"Time's Up! Start: {StartTime} | End: {EndTime}";
        public float DurationInSeconds { get; private set; }
        public float ElapsedTime => Math.Min(DurationInSeconds, Time.time - StartTime);
        public float TimeLeft =>  Math.Max(0f, DurationInSeconds - ElapsedTime);
        public float StartTime { get; private set; }
        public float EndTime { get; private set; }

        public TimerRule(float durationInSeconds = 60f)
        {
            DurationInSeconds = durationInSeconds;
        }

        public bool IsGameOver(IBoardService boardService, IScoreService scoreService)
        {
            return ElapsedTime >= DurationInSeconds;
        }

        public async UniTask StartAsync(IBoardService boardService, IScoreService scoreService, 
            Action<IGameEndRule> onGameEnd)
        {
            StartTime = Time.time;
            await UniTask.Delay(TimeSpan.FromSeconds(DurationInSeconds));
            EndTime = Time.time;
            
            onGameEnd?.Invoke(this);
        }
    }
}