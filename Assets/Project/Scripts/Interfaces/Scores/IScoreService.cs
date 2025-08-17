using System;
using Gazeus.DesafioMatch3.Models;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Scores
{
    public interface IScoreService : IDisposable
    {
        event Action<int> ScoreUpdated;
        int CurrentScore { get; }

        public void SetScore(int newScore);
        BoardSequenceScoreInfo CalculateSequenceScore(BoardSequence boardSequence, int index);
    }
}