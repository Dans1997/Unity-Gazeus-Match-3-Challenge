using System;
using System.Collections.Generic;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;
using Sirenix.Serialization;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct GameEndResults
    {
        [OdinSerialize] public int FinalScore { get; set; }
        [OdinSerialize] public float FinalTimeInSeconds { get; set; }
        [OdinSerialize] public IReadOnlyCollection<IGameEndRule> TriggeredEndRules { get; set; }
    }
}