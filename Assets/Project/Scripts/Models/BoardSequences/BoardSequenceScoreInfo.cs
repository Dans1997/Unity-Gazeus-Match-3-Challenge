using System;
using Sirenix.Serialization;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct BoardSequenceScoreInfo
    {
        [OdinSerialize] public int OldScore { get; private set; }
        [OdinSerialize] public int NewScore { get; private set; }
        
        public BoardSequenceScoreInfo(int oldScore, int newScore)
        {
            OldScore = oldScore;
            NewScore = newScore;
        }
    }
}