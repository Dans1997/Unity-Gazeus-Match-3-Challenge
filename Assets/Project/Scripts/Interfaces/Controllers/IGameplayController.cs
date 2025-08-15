using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Views;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers
{
    public interface IGameplayController : IController
    {
        GameplayInfo GameplayInfo { get; }
        IAssetProvider AssetProvider { get; }
        IGameplayService GameplayService { get; }
        BoardView BoardView { get; }
        public IBoardCellView SelectedCellView { get; }
        bool IsAnimating { get; }
        int SelectedX { get; }
        int SelectedY { get; }
    }
}