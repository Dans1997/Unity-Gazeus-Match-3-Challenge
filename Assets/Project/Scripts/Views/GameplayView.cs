using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Models.BoardTiles;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Scripts.Helpers;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles;
using Gazeus.Match3Challenge.Project.Scripts.Models.BoardTiles;
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
        public event Action<IBoardTileView> TileDestroyed;
        public event Action HintButtonClicked;
        public event Action QuitGameButtonClicked;
        
        public Transform Transform => transform;
        
        [FoldoutGroup("Board")] [OdinSerialize, ReadOnly] private GridLayoutGroup _boardContainer;
        [FoldoutGroup("Board")] [OdinSerialize, ReadOnly] private IBoardTileView[][] _tiles;
        [FoldoutGroup("Board")] [OdinSerialize, ReadOnly] private IBoardCellView[][] _boardCells;
        [FoldoutGroup("Board")] [OdinSerialize, ReadOnly] private BoardTileLoadedAssets[] _tilePrefabInfos;
        [FoldoutGroup("Score")] [OdinSerialize] private TMP_Text scoreText;
        [FoldoutGroup("Score")] [OdinSerialize, ReadOnly] private string scoreFormat;
        [FoldoutGroup("Time")] [OdinSerialize] private CanvasGroup timeCanvasGroup;
        [FoldoutGroup("Time")] [OdinSerialize] private TMP_Text timeText;
        [FoldoutGroup("Buttons")] [OdinSerialize] private Button hintButton;
        [FoldoutGroup("Buttons")] [OdinSerialize] private Button quitGameButton;
        [FoldoutGroup("Prefabs")] [OdinSerialize, ReadOnly] private IBoardCellView _boardCellPrefab;
        [FoldoutGroup("Prefabs")] [OdinSerialize, ReadOnly] private IBoardTileView _boardTilePrefab;

        private void Awake()
        {
            _boardContainer = GetComponentInChildren<GridLayoutGroup>();
        }

        private void Start()
        {
            _boardContainer.transform.DestroyAllChildren();
            hintButton.onClick.AddListener(OnHintButtonClicked);
            quitGameButton.onClick.AddListener(OnQuitGameButtonClicked);
        }

        private void OnDestroy()
        {
            hintButton.onClick.RemoveListener(OnHintButtonClicked);
            quitGameButton.onClick.RemoveListener(OnQuitGameButtonClicked);
        }

        public void ConfigureBoardVisuals(BoardVisualConfig config,
            BoardTileLoadedAssets[] boardTileLoadedAssets, IBoardCellView boardCellViewPrefab,
            IBoardTileView boardTileViewPrefab, int constraintCount)
        {
            _boardContainer.cellSize = config.CellSize;
            _boardContainer.spacing = config.Spacing;
            _boardContainer.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _boardContainer.constraintCount = constraintCount;
            _boardCellPrefab = boardCellViewPrefab;
            _boardTilePrefab = boardTileViewPrefab;
            _tilePrefabInfos = boardTileLoadedAssets;
            scoreFormat = config.ScoreFormat;
        }

        public async UniTask BuildBoardVisuals(IReadOnlyList<IReadOnlyList<BoardTileInfo>> board)
        {
            _tiles = new IBoardTileView[board.Count][];
            _boardCells = new IBoardCellView[board.Count][];
            
            for (var y = 0; y < board.Count; y++)
            {
                _tiles[y] = new IBoardTileView[board[0].Count];
                _boardCells[y] = new IBoardCellView[board[0].Count];

                for (var x = 0; x < board[0].Count; x++)
                {
                    var boardCell = LeanPool.Spawn(_boardCellPrefab.Transform, _boardContainer.transform)
                        .GetComponent<IBoardCellView>();
                    boardCell.SetPosition(new Vector2Int(x, y));
                    boardCell.Clicked += OnBoardCellClicked;

                    _boardCells[y][x] = boardCell;

                    var tileTypeIndex = (int) board[y][x].Key;
                    if (tileTypeIndex <= -1) continue;
                    
                    var tile = SpawnBoardTileView(boardCell, board[y][x].Id, board[y][x].Key);

                    _tiles[y][x] = tile;
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }

        public Tween CreateTile(IReadOnlyList<AddedTileInfo> addedTiles)
        {
            var sequence = DOTween.Sequence();
            for (var i = 0; i < addedTiles.Count; i++)
            {
                var addedTileInfo = addedTiles[i];
                var position = addedTileInfo.Position;
                var boardCell = _boardCells[position.y][position.x];
                var tile = SpawnBoardTileView(boardCell, addedTileInfo.Id, addedTileInfo.Key);

                _tiles[position.y][position.x] = tile;

                tile.Transform.localScale = Vector2.zero;
                sequence.Join(tile.Transform.DOScale(1.0f, 0.2f));
            }

            return sequence;
        }

        public Tween DestroyTiles(IReadOnlyList<Vector2Int> matchedPositions)
        {
            foreach (var matchedPosition in matchedPositions)
            {
                var boardTileView = _tiles[matchedPosition.y][matchedPosition.x];
                var destructionObject = boardTileView.PlayDestructionSequence();
                destructionObject.DespawnAfterDelay(2f); // TODO: Hardcoded. Pass duration later
                LeanPool.Despawn(boardTileView.Transform);
                _tiles[matchedPosition.y][matchedPosition.x] = null;
            }

            return DOVirtual.DelayedCall(0.5f, () => { });
        }

        public Tween MoveTiles(IReadOnlyList<MovedTileInfo> movedTiles)
        {
            var tiles = new IBoardTileView[_tiles.Length][];
            for (var y = 0; y < _tiles.Length; y++)
            {
                tiles[y] = new IBoardTileView[_tiles[y].Length];
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

                sequence.Join(_boardCells[to.y][to.x].AnimatedSetTile(_tiles[from.y][from.x].Transform));

                tiles[to.y][to.x] = _tiles[from.y][from.x];
            }

            _tiles = tiles;

            return sequence;
        }

        public Tween SwapTiles(int fromX, int fromY, int toX, int toY)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(_boardCells[fromY][fromX].AnimatedSetTile(_tiles[toY][toX].Transform));
            sequence.Join(_boardCells[toY][toX].AnimatedSetTile(_tiles[fromY][fromX].Transform));

            (_tiles[toY][toX], _tiles[fromY][fromX]) = (_tiles[fromY][fromX], _tiles[toY][toX]);

            return sequence;
        }

        public Tween UpdateScore(BoardSequenceScoreInfo boardSequenceScoreInfo, float duration = 0.25f)
        {
            return DOTween.To(() => boardSequenceScoreInfo.OldScore, value =>
            {
                scoreText.text = value.ToString(format: scoreFormat);
            }, 
            boardSequenceScoreInfo.NewScore, duration).SetEase(Ease.OutCubic);
        }

        public void UpdateTime(float timeLeft)
        {
            timeCanvasGroup.alpha = 1f;
            timeText.text = timeLeft.FormatTime();
        }

        public async void HighlightBoardMatch(ValidTileMoveInfo validMoveInfo, float duration = 3) 
        {
            if (_boardCells == null) return;
            var validPositions = new List<Vector2Int>();
            var validCells = new List<IBoardCellView>();
            
            validPositions.Add(validMoveInfo.From);
            validPositions.Add(validMoveInfo.To);
            
            foreach (var position in validPositions) 
            {
                var row = _boardCells[position.y];
                var boardCellView = row[position.x];
                validCells.Add(boardCellView);
                boardCellView.SetSelected(true);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(duration));

            foreach (var cell in validCells)
            {
                cell.SetSelected(false);
            }
        }

        private IBoardTileView SpawnBoardTileView(IBoardCellView boardCell, int id, TileKey tileKey)
        {
            var tilePrefabInfo = _tilePrefabInfos[(int)tileKey];
            var tile = LeanPool.Spawn(_boardTilePrefab.Transform).GetComponent<IBoardTileView>();
            tile.Transform.gameObject.name = $"{tileKey} #{id}";
            tile.ConfigureTileVisuals(tilePrefabInfo);
            boardCell.SetTile(tile.Transform);
            return tile;
        }

        private void OnBoardCellClicked(IBoardCellView position) => TileClicked?.Invoke(position);
        private void OnHintButtonClicked() => HintButtonClicked?.Invoke();
        private void OnQuitGameButtonClicked() => QuitGameButtonClicked?.Invoke();
    }
}
