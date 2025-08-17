using System;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers
{
    public interface IGameplayController : IController
    {
        event Action GameStarted;
        event Action TileClicked;
        event Action TileSelected;
        event Action TileSwapped;
        event Action ScoreUpdated;
        event Action<GameEndResults> GameEnded;
        
        GameplayInfo GameplayInfo { get; }
        IAssetLoadService AssetLoadService { get; }
        IBoardService BoardService { get; }
        IGameplayView GameplayView { get; }
        IBoardCellView SelectedCellView { get; }
        bool IsAnimating { get; }
        int SelectedX { get; }
        int SelectedY { get; }
        
        void StartGame();
    }
}