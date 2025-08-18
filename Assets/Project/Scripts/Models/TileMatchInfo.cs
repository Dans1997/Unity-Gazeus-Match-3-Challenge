using System.Collections.Generic;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    public class TileMatchInfo : ITileMatchInfo
    {
        public TileMatchType TileMatchType { get; }
        public TileKey? Key { get; }
        public IReadOnlyList<Vector2Int> Positions { get; }
        
        public TileMatchInfo(TileMatchType type, TileKey? key, IReadOnlyList<Vector2Int> positions)
        {
            TileMatchType = type;
            Key = key;
            Positions = positions;
        }
    }
}