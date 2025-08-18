using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Rules.Match
{
    public class SquareShapedTileMatchRule : ITileMatchRule
    {
        public TileMatchType TileMatchType => TileMatchType.SquareShaped;
        
        private readonly int size;

        public SquareShapedTileMatchRule(int size)
        {
            this.size = Mathf.Max(2, size);
        }
        
        public List<ITileMatchInfo> FindMatches(IReadOnlyList<IReadOnlyList<TileInfo>> board)
        {
            var matches = new List<ITileMatchInfo>();

            var height = board.Count;
            var width = board[0].Count;
            if (height < size || width < size) return matches;

            for (var y = 0; y <= height - size; y++)
            {
                for (var x = 0; x <= width - size; x++)
                {
                    var key = board[y][x].Key;
                    if (key.Equals((TileKey)(-1))) continue;

                    var allSame = true;
                    for (var yy = 0; yy < size && allSame; yy++)
                    {
                        for (var xx = 0; xx < size; xx++)
                        {
                            if (board[y + yy][x + xx].Key == key) continue;
                            allSame = false;
                            break;
                        }
                    }

                    if (!allSame) continue;

                    var positions = new List<Vector2Int>(size * size);
                    for (var yy = 0; yy < size; yy++)
                    {
                        for (var xx = 0; xx < size; xx++)
                        {
                            positions.Add(new Vector2Int(x + xx, y + yy));
                        }
                    }

                    matches.Add(new TileMatchInfo(TileMatchType.SquareShaped, key, positions));
                }
            }

            return matches;
        }
    }
}