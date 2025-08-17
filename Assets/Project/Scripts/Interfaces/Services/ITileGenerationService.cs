using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface ITileGenerationService
    {
        public void GenerateNextTile(TileInfo tileInfo, IReadOnlyList<TileKey> tileKeys, IBoardState boardState);
    }
}