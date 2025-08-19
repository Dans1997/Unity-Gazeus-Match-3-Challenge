using System;
using System.Linq;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Services
{
    public class DefaultTileGenerationService : ITileGenerationService
    {
        private readonly BoardTileConfig[] boardTileConfigs;
        private readonly TileKey[] availableTileKeys;
        private readonly int[] cumulative;
        private readonly int totalWeight;

        public DefaultTileGenerationService(GameplayInfo gameplayInfo)
        {
            this.boardTileConfigs = gameplayInfo.AvailableTileConfigs;
            availableTileKeys = boardTileConfigs.Select(c => c.TileKey).ToArray();
            cumulative = new int[boardTileConfigs.Length];

            var sum = 0;
            for (var i = 0; i < boardTileConfigs.Length; i++)
            {
                var w = Mathf.Max(0, boardTileConfigs[i].Weight);
                sum += w;
                cumulative[i] = sum;
            }

            totalWeight = Mathf.Max(0, sum);
        }

        public BoardTileInfo GenerateNextTile(IBoardState boardState)
        {
            var r = Random.value * totalWeight;
            var idx = BinarySearchCumulative(r);
            var tileInfo = new BoardTileInfo(boardState.TileCount++, availableTileKeys[idx]);
            return tileInfo;
        }

        private int BinarySearchCumulative(float value)
        {
            var lo = 0;
            var hi = cumulative.Length - 1;
            while (lo < hi)
            {
                var mid = (lo + hi) >> 1;
                if (value <= cumulative[mid]) hi = mid;
                else lo = mid + 1;
            }
            return Math.Clamp(lo, 0, cumulative.Length - 1);
        }
    }
}