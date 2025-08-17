using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models
{
    public interface IBoardSwapResult
    {
        public IBoardState NewState { get; }
        public IReadOnlyList<BoardSequence> Sequences { get; }
        public int ScoreGained { get; } // TODO:
    }
}