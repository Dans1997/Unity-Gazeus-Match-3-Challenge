using System.Collections.Generic;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using Gazeus.Match3Challenge.Project.Scripts.Core.Services;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;

namespace Gazeus.DesafioMatch3.Core.Services
{
    public class BoardService : IBoardService
    {
        public GameplayInfo GameplayInfo { get; private set; }
        public List<List<TileInfo>> BoardTiles => boardTiles;
        public int TileCount => tileCount;

        private readonly ITileGenerationService tileGenerationService;
        private readonly IBoardCreationService boardCreationService;
        private readonly IMoveValidationService moveValidationService;
        private readonly ITileSwapService tileSwapService;
        private List<List<TileInfo>> boardTiles;
        private int tileCount;

        public BoardService(GameplayInfo gameplayInfo, ITileGenerationService tileGenerationService = null, 
            IBoardCreationService boardCreationService = null, IMatchFindService matchFindService = null, 
            IMoveValidationService moveValidationService = null, ITileSwapService tileSwapService = null)
        {
            GameplayInfo = gameplayInfo;
            this.tileGenerationService = tileGenerationService ?? new DefaultTileGenerationService();
            this.boardCreationService = boardCreationService ?? new DefaultBoardCreationService(gameplayInfo, this.tileGenerationService);
            this.moveValidationService = moveValidationService ?? new DefaultMoveValidationService();
            this.tileSwapService = tileSwapService ?? new DefaultTileSwapService(matchFindService, this.tileGenerationService, gameplayInfo.AvailableTileKeys);
        }

        public List<List<TileInfo>> CreateBoard() => boardCreationService.CreateBoard(ref boardTiles, ref tileCount);
        public bool IsValidMovement(int fromX, int fromY, int toX, int toY) => moveValidationService.IsValidMove(boardTiles, fromX, fromY, toX, toY);
        public bool HasAnyValidMove() => moveValidationService.HasAnyValidMove(boardTiles);
        public List<BoardSequence> SwapTile(int fromX, int fromY, int toX, int toY) => tileSwapService.SwapTile(ref boardTiles, ref tileCount, fromX, fromY, toX, toY);
    }
}
