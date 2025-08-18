using System;
using System.Collections.Generic;
using DG.Tweening;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Scripts.Helpers;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles;
using Lean.Pool;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    public class GameplayView : SerializedMonoBehaviour, IGameplayView
    {
        public event Action<IBoardCellView> TileClicked;
        public Transform Transform => transform;
        
        [FoldoutGroup("Board")] [OdinSerialize, ReadOnly] private GridLayoutGroup _boardContainer;
        [FoldoutGroup("Board")] [OdinSerialize, ReadOnly] private GameObject[][] _tiles;
        [FoldoutGroup("Board")] [OdinSerialize, ReadOnly] private IBoardCellView[][] _boardCells;
        [FoldoutGroup("Board")] [OdinSerialize, ReadOnly] private TilePrefabInfo[] _tilePrefabs;
        [FoldoutGroup("Score")] [OdinSerialize] private TMP_Text scoreText;
        [FoldoutGroup("Score")] [OdinSerialize, ReadOnly] private string scoreFormat;

        private void Awake()
        {
            _boardContainer = GetComponentInChildren<GridLayoutGroup>();
        }

        private void Start()
        {
            _boardContainer.transform.DestroyAllChildren();
        }

        public void ConfigureBoardVisuals(BoardVisualConfig config, int constraintCount)
        {
            _boardContainer.cellSize = config.CellSize;
            _boardContainer.spacing = config.Spacing;
            _boardContainer.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _boardContainer.constraintCount = constraintCount;
            scoreFormat = config.ScoreFormat;
        }

        public void BuildBoardVisuals(IReadOnlyList<IReadOnlyList<TileInfo>> board, IBoardCellView boardCellPrefab,
            TilePrefabInfo[] tiles)
        {
            _tiles = new GameObject[board.Count][];
            _boardCells = new IBoardCellView[board.Count][];
            _tilePrefabs = tiles;

            for (var y = 0; y < board.Count; y++)
            {
                _tiles[y] = new GameObject[board[0].Count];
                _boardCells[y] = new IBoardCellView[board[0].Count];

                for (var x = 0; x < board[0].Count; x++)
                {
                    var boardCell = LeanPool.Spawn(boardCellPrefab.Transform, _boardContainer.transform)
                        .GetComponent<IBoardCellView>();
                    boardCell.SetPosition(new Vector2Int(x, y));
                    boardCell.Clicked += OnBoardCellClicked;

                    _boardCells[y][x] = boardCell;

                    var tileTypeIndex = (int) board[y][x].Key;
                    if (tileTypeIndex <= -1) continue;
                    
                    var tilePrefab = tiles[tileTypeIndex].Prefab;
                    var tile = LeanPool.Spawn(tilePrefab);
                    boardCell.SetTile(tile.transform);

                    _tiles[y][x] = tile;
                }
            }
        }

        public Tween CreateTile(IReadOnlyList<AddedTileInfo> addedTiles)
        {
            var sequence = DOTween.Sequence();
            for (var i = 0; i < addedTiles.Count; i++)
            {
                var addedTileInfo = addedTiles[i];
                var position = addedTileInfo.Position;
                var boardCell = _boardCells[position.y][position.x];
                var tilePrefab = _tilePrefabs[(int)addedTileInfo.Key].Prefab;
                var tile = LeanPool.Spawn(tilePrefab);
                boardCell.SetTile(tile.transform);

                _tiles[position.y][position.x] = tile;

                tile.transform.localScale = Vector2.zero;
                sequence.Join(tile.transform.DOScale(1.0f, 0.2f));
            }

            return sequence;
        }

        public Tween DestroyTiles(IReadOnlyList<Vector2Int> matchedPosition)
        {
            for (var i = 0; i < matchedPosition.Count; i++)
            {
                var position = matchedPosition[i];
                LeanPool.Despawn(_tiles[position.y][position.x]);
                _tiles[position.y][position.x] = null;
            }

            return DOVirtual.DelayedCall(0.2f, () => { });
        }

        public Tween MoveTiles(IReadOnlyList<MovedTileInfo> movedTiles)
        {
            var tiles = new GameObject[_tiles.Length][];
            for (var y = 0; y < _tiles.Length; y++)
            {
                tiles[y] = new GameObject[_tiles[y].Length];
                for (var x = 0; x < _tiles[y].Length; x++)
                {
                    tiles[y][x] = _tiles[y][x];
                }
            }

            var sequence = DOTween.Sequence();
            for (var i = 0; i < movedTiles.Count; i++)
            {
                var movedTileInfo = movedTiles[i];

                var from = movedTileInfo.From;
                var to = movedTileInfo.To;

                sequence.Join(_boardCells[to.y][to.x].AnimatedSetTile(_tiles[from.y][from.x].transform));

                tiles[to.y][to.x] = _tiles[from.y][from.x];
            }

            _tiles = tiles;

            return sequence;
        }

        public Tween SwapTiles(int fromX, int fromY, int toX, int toY)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(_boardCells[fromY][fromX].AnimatedSetTile(_tiles[toY][toX].transform));
            sequence.Join(_boardCells[toY][toX].AnimatedSetTile(_tiles[fromY][fromX].transform));

            (_tiles[toY][toX], _tiles[fromY][fromX]) = (_tiles[fromY][fromX], _tiles[toY][toX]);

            return sequence;
        }

        public Tween UpdateScore(BoardSequenceScoreInfo boardSequenceScoreInfo, float duration = 0.25f)
        {
            return DOTween.To(() => boardSequenceScoreInfo.OldScore, value =>
            {
                boardSequenceScoreInfo.OldScore = value;
                scoreText.text = value.ToString(format: scoreFormat);
            }, 
            boardSequenceScoreInfo.NewScore, duration).SetEase(Ease.OutCubic);
        }

        private void OnBoardCellClicked(IBoardCellView position)
        {
            TileClicked?.Invoke(position);
        }
    }
}
