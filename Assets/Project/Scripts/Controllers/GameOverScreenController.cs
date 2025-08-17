using System;
using Cysharp.Threading.Tasks;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class GameOverScreenController : IGameOverScreenController
    {
        public event Action ReplayRequested;
        public event Action MainMenuRequested;
        
        public GameplayInfo GameplayInfo { get; private set; }
        public GameEndResults GameEndResults { get; private set; }
        public IAssetLoadService AssetLoadService { get; private set; }
        public IGameOverScreenView GameOverScreenView { get; private set; }

        public GameOverScreenController(GameplayInfo gameplayInfo, GameEndResults gameEndResults, 
            IAssetLoadService assetLoadService) 
        {
            GameplayInfo = gameplayInfo;
            GameEndResults = gameEndResults;
            AssetLoadService = assetLoadService;
        }
        
        public async UniTask Initialize()
        {
            GameOverScreenView = await AssetLoadService.InstantiateAsync<IGameOverScreenView>(GameplayInfo.GameOverViewPrefabKey);
            GameOverScreenView.SetGameEndResults(GameplayInfo, GameEndResults);
            await UniTask.Yield();
            GameOverScreenView.ReplayClicked += OnReplayClicked;
            GameOverScreenView.MainMenuClicked += OnMainMenuClicked;
        }

        public void Dispose()
        {
            if (GameOverScreenView == null) return;
            GameOverScreenView.ReplayClicked -= OnReplayClicked;
            GameOverScreenView.MainMenuClicked -= OnMainMenuClicked;
            AssetLoadService.Release(GameOverScreenView.Transform.gameObject);
            GameOverScreenView = null;
        }
        
        private void OnReplayClicked()
        {
            ReplayRequested?.Invoke();
        }

        private void OnMainMenuClicked()
        {
            MainMenuRequested?.Invoke();
        }
    }
}