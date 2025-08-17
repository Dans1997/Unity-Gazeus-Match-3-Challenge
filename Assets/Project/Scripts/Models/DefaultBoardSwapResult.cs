using System.Collections.Generic;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;

namespace Gazeus.DesafioMatch3.Models
{
    public class DefaultBoardSwapResult : IBoardSwapResult
    {
        public IBoardState NewState { get; }
        public IReadOnlyList<BoardSequence> Sequences { get; }
        public int ScoreGained { get; } 

        public DefaultBoardSwapResult(IBoardState newState, IReadOnlyList<BoardSequence> sequences, int scoreGained = 0)
        {
            NewState = newState;
            Sequences = sequences;
            ScoreGained = scoreGained;
        }
    }
}