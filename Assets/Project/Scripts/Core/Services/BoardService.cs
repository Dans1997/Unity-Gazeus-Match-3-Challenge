using System.Linq;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using Gazeus.Match3Challenge.Project.Scripts.Core.Rules.Match;
using Gazeus.Match3Challenge.Project.Scripts.Core.Services;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;
using Gazeus.Match3Challenge.Project.Scripts.Models.BoardTiles;

namespace Gazeus.DesafioMatch3.Core.Services
{
    public class BoardService : IBoardService
    {
        public GameplayInfo GameplayInfo { get; private set; }
        public ITileMatchRule[] MatchRules { get; private set; }
        public IBoardState BoardState { get; private set; }

        private readonly ITileGenerationService tileGenerationService;
        private readonly IMatchFindService matchFindService;
        private readonly IBoardCreationService boardCreationService;
        private readonly IMoveValidationService moveValidationService;
        private readonly ITileSwapService tileSwapService;
        private readonly IBoardEffectService boardEffectService;

        public BoardService(GameplayInfo gameplayInfo, ITileGenerationService tileGenerationService = null, 
            IBoardCreationService boardCreationService = null, IMatchFindService matchFindService = null, 
            IMoveValidationService moveValidationService = null, ITileSwapService tileSwapService = null,
            IBoardEffectService boardEffectService = null)
        {
            GameplayInfo = gameplayInfo;
            MatchRules = gameplayInfo.TileMatchRules    
                .Select(config => new TileMatchRuleFactory().Create(config))
                .ToArray();
            
            this.tileGenerationService = tileGenerationService ?? new DefaultTileGenerationService(GameplayInfo);
            this.matchFindService = matchFindService ?? new DefaultMatchFindService();
            this.boardEffectService = boardEffectService ?? new DefaultBoardEffectService(GameplayInfo.BoardEffectConfigs);
            
            this.boardCreationService = boardCreationService ?? new DefaultBoardCreationService(GameplayInfo, this.tileGenerationService);
            this.moveValidationService = moveValidationService ?? new DefaultMoveValidationService(this.matchFindService,
                MatchRules);
            this.tileSwapService = tileSwapService ?? new DefaultTileSwapService(MatchRules, this.matchFindService, 
                this.tileGenerationService, this.boardEffectService);
        }

        public void CreateBoard() => BoardState = boardCreationService.CreateBoard();
        public bool IsValidMove(int fromX, int fromY, int toX, int toY) => moveValidationService.IsValidMove(BoardState, fromX, fromY, toX, toY, out _);
        public bool HasAnyValidMove(out FindMatchResult findMatchResult)
        {
            throw new System.NotImplementedException();
        }

        public bool HasAnyValidMove(out ValidTileMoveInfo firstValidMove) => moveValidationService.HasAnyValidMove(BoardState, out firstValidMove);
        public IBoardSwapResult SwapTile(int fromX, int fromY, int toX, int toY) => tileSwapService.SwapTile(BoardState, fromX, fromY, toX, toY);
    }
}
