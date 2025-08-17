using System;
using Sirenix.Serialization;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct BoardSequenceScoreInfo
    {
        [OdinSerialize] public int OldScore { get; set; }
        [OdinSerialize] public int NewScore { get; set; }
    }
}