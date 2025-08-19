using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface IMatchFindService
    {
        public FindMatchResult FindMatches(IReadOnlyList<IReadOnlyList<BoardTileInfo>> board, 
            IReadOnlyList<ITileMatchRule> matchRules);
    }
}