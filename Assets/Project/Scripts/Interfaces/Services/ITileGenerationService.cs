using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface ITileGenerationService
    {
        public void GenerateNextTile(TileInfo tileInfo, IReadOnlyList<TileKey> tileKeys, ref int tileCount);
    }
}