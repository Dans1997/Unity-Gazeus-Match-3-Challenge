using System.Linq;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using Gazeus.Match3Challenge.Project.Scripts.Core.Rules.Match;
using Gazeus.Match3Challenge.Project.Scripts.Core.Services;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Rules;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services;

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

        public BoardService(GameplayInfo gameplayInfo, ITileGenerationService tileGenerationService = null, 
            IBoardCreationService boardCreationService = null, IMatchFindService matchFindService = null, 
            IMoveValidationService moveValidationService = null, ITileSwapService tileSwapService = null)
        {
            GameplayInfo = gameplayInfo;
            MatchRules = gameplayInfo.TileMatchRules    
                .Select(config => new TileMatchRuleFactory().Create(config))
                .ToArray();
            
            this.tileGenerationService = tileGenerationService ?? new DefaultTileGenerationService(GameplayInfo);
            this.matchFindService = matchFindService ?? new DefaultMatchFindService();
            
            this.boardCreationService = boardCreationService ?? new DefaultBoardCreationService(GameplayInfo, this.tileGenerationService);
            this.moveValidationService = moveValidationService ?? new DefaultMoveValidationService(this.matchFindService,
                MatchRules);
            this.tileSwapService = tileSwapService ?? new DefaultTileSwapService(GameplayInfo, this.matchFindService, 
                this.tileGenerationService, MatchRules);
        }

        public void CreateBoard() => BoardState = boardCreationService.CreateBoard();
        public bool IsValidMovement(int fromX, int fromY, int toX, int toY) => moveValidationService.IsValidMove(BoardState, fromX, fromY, toX, toY);
        public bool HasAnyValidMove() => moveValidationService.HasAnyValidMove(BoardState);
        public IBoardSwapResult SwapTile(int fromX, int fromY, int toX, int toY) => tileSwapService.SwapTile(BoardState, fromX, fromY, toX, toY);
    }
}
