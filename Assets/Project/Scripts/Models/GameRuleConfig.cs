using System;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct GameRuleConfig
    {
        [OdinSerialize] public GameRuleKey RuleKey { get; private set; }

        [ShowIf(nameof(RuleKey), GameRuleKey.ScoreThresholdRule)] 
        [OdinSerialize] public int ScoreThreshold { get; private set; }

        [ShowIf(nameof(RuleKey), GameRuleKey.TimerRule)] 
        [OdinSerialize] public float TimerSeconds { get; private set; }
    }
}