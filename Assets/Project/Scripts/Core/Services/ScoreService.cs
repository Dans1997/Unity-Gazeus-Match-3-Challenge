using System;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Scores;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Services
{
    public class ScoreService : IScoreService
    {
        public event Action<int> ScoreUpdated;
        public int CurrentScore { get; private set; }

        public void Dispose()
        {
            // TODO: release managed resources here
        }

        public void SetScore(int newScore)
        {
            CurrentScore = newScore;
            ScoreUpdated?.Invoke(CurrentScore);
        }
        
        public BoardSequenceScoreInfo CalculateSequenceScore(BoardSequence boardSequence, int index)
        {
            var matchDepth = index + 1;
            var matchSize = boardSequence.MatchedPosition.Count;
            var scoreDelta = matchSize + matchDepth;
            var oldScore = CurrentScore;
            var newScore = CurrentScore + scoreDelta;

            SetScore(newScore);
            return new BoardSequenceScoreInfo
            {
                OldScore = oldScore,
                NewScore = newScore
            };
        }
    }
}