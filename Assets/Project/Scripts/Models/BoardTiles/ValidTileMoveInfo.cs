using System;
using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Models.BoardTiles
{
    [Serializable]
    public class ValidTileMoveInfo
    {
        public Vector2Int From { get; private set; }
        public Vector2Int To { get; private set; }
        public FindMatchResult MatchResult { get; private set; }

        public ValidTileMoveInfo(Vector2Int from, Vector2Int to, FindMatchResult matchResult)
        {
            From = from;
            To = to;
            MatchResult = matchResult;
        }
    }
}