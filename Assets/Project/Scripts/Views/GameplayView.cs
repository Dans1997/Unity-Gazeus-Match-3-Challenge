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
        
        [FoldoutGroup("Board")] [OdinSerialize, ReadOnly] public GridLayoutGroup BoardContainer { get; private set; }
        [FoldoutGroup("Board")] [OdinSerialize, ReadOnly] public IBoardTileView[][] TileViews { get; private set; }
        [FoldoutGroup("Board")] [OdinSerialize, ReadOnly] public IBoardCellView[][] BoardCellViews { get; private set; }
        [FoldoutGroup("Board")] [OdinSerialize, ReadOnly] public BoardTileLoadedAssets[] LoadedAssets { get; private set; }
        [FoldoutGroup("Score")] [OdinSerialize] public TMP_Text ScoreText { get; private set; }
        [FoldoutGroup("Score")] [OdinSerialize, ReadOnly] public string ScoreFormat { get; private set; }
        [FoldoutGroup("Timer")] [OdinSerialize] public CanvasGroup TimeCanvasGroup { get; private set; }
        [FoldoutGroup("Timer")] [OdinSerialize] public TMP_Text TimeText { get; private set; }
        [FoldoutGroup("Buttons")] [OdinSerialize] public Button HintButton { get; private set; }
        [FoldoutGroup("Buttons")] [OdinSerialize] public Button QuitGameButton { get; private set; }
        [FoldoutGroup("Prefabs")] [OdinSerialize, ReadOnly] public IBoardCellView BoardCellPrefab { get; private set; }
        [FoldoutGroup("Prefabs")] [OdinSerialize, ReadOnly] public IBoardTileView BoardTilePrefab { get; private set; }

        private void Awake()
        {
            BoardContainer = GetComponentInChildren<GridLayoutGroup>();
        }

        private void Start()
        {
            BoardContainer.transform.DestroyAllChildren();
            HintButton.onClick.AddListener(OnHintButtonClicked);
            QuitGameButton.onClick.AddListener(OnQuitGameButtonClicked);
        }

        private void OnDestroy()
        {
            HintButton.onClick.RemoveListener(OnHintButtonClicked);
            QuitGameButton.onClick.RemoveListener(OnQuitGameButtonClicked);
        }

        public void ConfigureBoardVisuals(BoardVisualConfig config,
            BoardTileLoadedAssets[] boardTileLoadedAssets, IBoardCellView boardCellViewPrefab,
            IBoardTileView boardTileViewPrefab, int constraintCount)
        {
            BoardContainer.cellSize = config.CellSize;
            BoardContainer.spacing = config.Spacing;
            BoardContainer.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            BoardContainer.constraintCount = constraintCount;
            BoardCellPrefab = boardCellViewPrefab;
            BoardTilePrefab = boardTileViewPrefab;
            LoadedAssets = boardTileLoadedAssets;
            ScoreFormat = config.ScoreFormat;
        }

        public async UniTask BuildBoardVisuals(IReadOnlyList<IReadOnlyList<BoardTileInfo>> board)
        {
            TileViews = new IBoardTileView[board.Count][];
            BoardCellViews = new IBoardCellView[board.Count][];
            
            for (var y = 0; y < board.Count; y++)
            {
                TileViews[y] = new IBoardTileView[board[0].Count];
                BoardCellViews[y] = new IBoardCellView[board[0].Count];

                for (var x = 0; x < board[0].Count; x++)
                {
                    var boardCell = LeanPool.Spawn(BoardCellPrefab.Transform, BoardContainer.transform)
                        .GetComponent<IBoardCellView>();
                    boardCell.SetPosition(new Vector2Int(x, y));
                    boardCell.Clicked += OnBoardCellClicked;

                    BoardCellViews[y][x] = boardCell;

                    var tileTypeIndex = (int) board[y][x].Key;
                    if (tileTypeIndex <= -1) continue;
                    
                    var tile = SpawnBoardTileView(boardCell, board[y][x].Id, board[y][x].Key);

                    TileViews[y][x] = tile;
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }

        public Tween CreateTile(IReadOnlyList<AddedTileInfo> addedTiles)
        {
            var sequence = DOTween.Sequence();
            foreach (var addedTileInfo in addedTiles)
            {
                var position = addedTileInfo.Position;
                var boardCell = BoardCellViews[position.y][position.x];
                var tile = SpawnBoardTileView(boardCell, addedTileInfo.Id, addedTileInfo.Key);

                TileViews[position.y][position.x] = tile;

                tile.Transform.localScale = Vector2.zero;
                sequence.Join(tile.Transform.DOScale(1.0f, 0.2f));
            }

            return sequence;
        }

        public Tween DestroyTiles(IReadOnlyList<Vector2Int> matchedPositions)
        {
            foreach (var matchedPosition in matchedPositions)
            {
                var boardTileView = TileViews[matchedPosition.y][matchedPosition.x];
                var destructionObject = boardTileView.PlayDestructionSequence();
                destructionObject.DespawnAfterDelay(2f); // TODO: Hardcoded. Pass duration later
                LeanPool.Despawn(boardTileView.Transform);
                TileViews[matchedPosition.y][matchedPosition.x] = null;
            }

            return DOVirtual.DelayedCall(0.5f, () => { });
        }

        public Tween MoveTiles(IReadOnlyList<MovedTileInfo> movedTiles)
        {
            var tiles = new IBoardTileView[TileViews.Length][];
            for (var y = 0; y < TileViews.Length; y++)
            {
                tiles[y] = new IBoardTileView[TileViews[y].Length];
                for (var x = 0; x < TileViews[y].Length; x++)
                {
                    tiles[y][x] = TileViews[y][x];
                }
            }

            var sequence = DOTween.Sequence();
            foreach (var movedTileInfo in movedTiles)
            {
                var from = movedTileInfo.From;
                var to = movedTileInfo.To;

                sequence.Join(BoardCellViews[to.y][to.x].SetTileAnimated(TileViews[from.y][from.x].Transform));

                tiles[to.y][to.x] = TileViews[from.y][from.x];
            }

            TileViews = tiles;

            return sequence;
        }

        public Tween SwapTiles(int fromX, int fromY, int toX, int toY)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(BoardCellViews[fromY][fromX].SetTileAnimated(TileViews[toY][toX].Transform));
            sequence.Join(BoardCellViews[toY][toX].SetTileAnimated(TileViews[fromY][fromX].Transform));

            (TileViews[toY][toX], TileViews[fromY][fromX]) = (TileViews[fromY][fromX], TileViews[toY][toX]);

            return sequence;
        }

        public Tween UpdateScore(BoardSequenceScoreInfo boardSequenceScoreInfo, float duration = 0.25f)
        {
            return DOTween.To(() => boardSequenceScoreInfo.OldScore, value =>
            {
                ScoreText.text = value.ToString(format: ScoreFormat);
            }, 
            boardSequenceScoreInfo.NewScore, duration).SetEase(Ease.OutCubic);
        }

        public void UpdateTime(float timeLeft)
        {
            TimeCanvasGroup.alpha = 1f;
            TimeText.text = timeLeft.FormatTime();
        }

        public async void HighlightBoardMatch(ValidTileMoveInfo validMoveInfo, float duration = 3) 
        {
            if (BoardCellViews == null) return;
            var validPositions = new List<Vector2Int>();
            var validCells = new List<IBoardCellView>();
            
            validPositions.Add(validMoveInfo.From);
            validPositions.Add(validMoveInfo.To);
            
            foreach (var position in validPositions) 
            {
                var row = BoardCellViews[position.y];
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
            var tilePrefabInfo = LoadedAssets[(int)tileKey];
            var tile = LeanPool.Spawn(BoardTilePrefab.Transform).GetComponent<IBoardTileView>();
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
