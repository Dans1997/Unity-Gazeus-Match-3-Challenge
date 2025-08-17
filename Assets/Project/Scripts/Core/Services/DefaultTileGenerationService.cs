using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Services
{
    public class DefaultTileGenerationService : ITileGenerationService
    {
        public void GenerateNextTile(TileInfo tileInfo, IReadOnlyList<TileKey> tileKeys, IBoardState boardState)
        {
            tileInfo.Id = boardState.TileCount++;
            tileInfo.Key = tileKeys[Random.Range(0, tileKeys.Count)];
        }
    }
}