using System;
using Cysharp.Threading.Tasks;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.DesafioMatch3.Views;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class MainMenuController : IMainMenuController
    {
        public event Action PlayRequested;
        public event Action ExitRequested;
        public IAssetProvider AssetProvider { get; private set; }
        private readonly MainMenuViewKey mainMenuViewKey;
        private MainMenuView mainMenuView;

        public MainMenuController(IAssetProvider assetProvider, MainMenuViewKey mainMenuViewKey)
        {
            AssetProvider = assetProvider;
            this.mainMenuViewKey = mainMenuViewKey;
        }
        
        public async UniTask Initialize()
        {
            mainMenuView = await AssetProvider.InstantiateAsync<MainMenuView>(mainMenuViewKey);
            mainMenuView.PlayButtonClicked += OnPlayClicked;
            mainMenuView.ExitButtonClicked += OnExitClicked;
        }
        
        public void Dispose()
        {
            if (mainMenuView == null) return;
            mainMenuView.PlayButtonClicked -= OnPlayClicked;
            mainMenuView.ExitButtonClicked -= OnExitClicked;
            AssetProvider.Release(mainMenuView.gameObject);
            mainMenuView = null;
        }

        private void OnPlayClicked()
        {
            PlayRequested?.Invoke();
        }
        
        private void OnExitClicked()
        {
            ExitRequested?.Invoke();
        }
    }
}