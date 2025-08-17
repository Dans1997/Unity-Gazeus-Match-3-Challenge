using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Services
{
    public class DefaultTileGenerationService : ITileGenerationService
    {
        public void GenerateNextTile(TileInfo tileInfo, IReadOnlyList<TileKey> tileKeys, ref int tileCount)
        {
            tileInfo.Id = tileCount++;
            tileInfo.Key = tileKeys[Random.Range(0, tileKeys.Count)];
        }
    }
}