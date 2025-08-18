using System.Collections.Generic;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models
{
    public interface ITileMatchInfo
    {
        TileMatchType TileMatchType { get; }
        TileKey? Key { get; }
        IReadOnlyList<Vector2Int> Positions { get; }
    }
}