using System;
using System.Collections.Generic;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;
using Sirenix.Serialization;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public class GameEndResults
    {
        [OdinSerialize] public int FinalScore { get; private set; }
        [OdinSerialize] public float FinalTimeInSeconds { get; private set; }
        [OdinSerialize] public IReadOnlyCollection<IGameEndRule> TriggeredEndRules { get; private set; }
        
        public GameEndResults(int finalScore, float finalTimeInSeconds, IReadOnlyCollection<IGameEndRule> triggeredEndRules)
        {
            FinalScore = finalScore;
            FinalTimeInSeconds = finalTimeInSeconds;
            TriggeredEndRules = triggeredEndRules;
        }
    }
}