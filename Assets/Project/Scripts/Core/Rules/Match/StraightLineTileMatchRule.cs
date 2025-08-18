using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Rules.Match
{
    public class StraightLineTileMatchRule : ITileMatchRule
    {
        public TileMatchType TileMatchType => TileMatchType.StraightLine;
        
        private readonly int minLength;

        public StraightLineTileMatchRule(int minLength = 3)
        {
            this.minLength = minLength;
        }

         public List<ITileMatchInfo> FindMatches(IReadOnlyList<IReadOnlyList<TileInfo>> board)
        {
            var matches = new List<ITileMatchInfo>();
            if (board == null || board.Count == 0) return matches;

            var height = board.Count;
            var width = board[0].Count;

            FindHorizontalMatches(board, height, width, matches);
            FindVerticalMatches(board, height, width, matches);

            return matches;
        }

        private void FindHorizontalMatches(IReadOnlyList<IReadOnlyList<TileInfo>> board, int height, int width, List<ITileMatchInfo> outMatches)
        {
            for (var y = 0; y < height; y++)
            {
                var x = 0;
                while (x < width)
                {
                    var start = x;
                    var startKey = board[y][x].Key;
                    if (startKey.Equals((TileKey)(-1)))
                    {
                        x++;
                        continue;
                    }

                    x++;
                    while (x < width && board[y][x].Key == startKey) x++;

                    var runLength = x - start;
                    if (runLength < minLength) continue;
                    var positions = new List<Vector2Int>(runLength);
                    for (var ix = start; ix < x; ix++) positions.Add(new Vector2Int(ix, y));
                    outMatches.Add(new TileMatchInfo(TileMatchType.StraightLine, startKey, positions));
                }
            }
        }

        private void FindVerticalMatches(IReadOnlyList<IReadOnlyList<TileInfo>> board, int height, int width, List<ITileMatchInfo> outMatches)
        {
            for (var x = 0; x < width; x++)
            {
                var y = 0;
                while (y < height)
                {
                    var start = y;
                    var startKey = board[y][x].Key;
                    if (startKey.Equals((TileKey)(-1)))
                    {
                        y++;
                        continue;
                    }

                    y++;
                    while (y < height && board[y][x].Key == startKey) y++;

                    var runLength = y - start;
                    if (runLength < minLength) continue;
                    var positions = new List<Vector2Int>(runLength);
                    for (var iy = start; iy < y; iy++) positions.Add(new Vector2Int(x, iy));
                    outMatches.Add(new TileMatchInfo(TileMatchType.StraightLine, startKey, positions));
                }
            }
        }
    }
}