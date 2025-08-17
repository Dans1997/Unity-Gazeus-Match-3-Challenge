using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface IMatchFindService
    {
        List<List<bool>> FindMatches(List<List<TileInfo>> board);
    }
}