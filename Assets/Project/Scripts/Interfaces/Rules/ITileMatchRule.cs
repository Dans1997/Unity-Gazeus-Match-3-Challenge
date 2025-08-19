using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules
{
    public interface ITileMatchRule
    {
        TileMatchType TileMatchType { get; }
        List<ITileMatchInfo> FindMatches(IReadOnlyList<IReadOnlyList<BoardTileInfo>> board);
    }
}