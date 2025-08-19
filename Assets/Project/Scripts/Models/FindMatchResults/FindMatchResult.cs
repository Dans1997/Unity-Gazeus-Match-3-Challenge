using System;
using System.Collections.Generic;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Sirenix.Serialization;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public class FindMatchResult
    {
        [OdinSerialize] public IReadOnlyList<ITileMatchInfo> Matches { get; private set; }
        [OdinSerialize] public HashSet<Vector2Int> MatchedPositions { get; private set; }

        public FindMatchResult(IReadOnlyList<ITileMatchInfo> matches, HashSet<Vector2Int> matchedPositions)
        {
            Matches = matches;
            MatchedPositions = matchedPositions;
        }
    }
}