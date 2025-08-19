using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gazeus.DesafioMatch3.Controllers;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Configs;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using Sirenix.Serialization;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Core.GameInitialization
{
    [Serializable]
    public class GameFlowController : IGameFlowController
    {
        [OdinSerialize] public IGameConfig GameConfig { get; private set; }
        [OdinSerialize] public IAssetLoadService AssetLoadService { get; private set; }
        [OdinSerialize] public IControllerLoadService ControllerLoadService { get; private set; }
        [OdinSerialize] public IAudioController AudioController { get; private set; }
        [OdinSerialize] public IMainMenuController MainMenuController { get; private set; }
        [OdinSerialize] public IGameplayController GameplayController { get; private set; }
        [OdinSerialize] public IGameOverScreenController GameOverScreenController { get; private set; }
        [OdinSerialize] public ILoadingScreenController LoadingScreenController { get; private set; }

        public GameFlowController(IGameConfig gameConfig, IAssetLoadService assetLoadService, 
            IControllerLoadService controllerLoadService)
        {
            GameConfig = gameConfig;
            AssetLoadService = assetLoadService;
            ControllerLoadService = controllerLoadService;
        }

        public async UniTask StartGameAsync()
        {
            AudioController = await ControllerLoadService.LoadAsync(() =>
            {
                var controller = new AudioController(GameConfig.AudioControllerConfig, AssetLoadService);
                return controller;
            });
            
            LoadingScreenController = await LoadLoadingScreenAsync();
            await ShowMainMenuAsync();
        }
        
        public void Dispose()
        {
            UnloadMainMenu();
            UnloadGameplay();
            UnloadLoadingScreen();
            
            AudioController?.Dispose();
            MainMenuController?.Dispose();
            GameplayController?.Dispose();
            GameOverScreenController?.Dispose();
            LoadingScreenController?.Dispose();
        }

        private async UniTask<IGameplayController> LoadGameplayAsync()
        {
            return await ControllerLoadService.LoadAsync(() =>
            {
                var controller = new GameplayController(GameConfig.GameplayInfo, AssetLoadService);
                return controller;
            });
        }

        private void UnloadGameplay()
        {
            if (GameplayController == null) return;
            ControllerLoadService.Unload(GameplayController);
        }

        private async UniTask<IGameOverScreenController> LoadGameOverScreenAsync(GameEndResults gameEndResults)
        {
            return await ControllerLoadService.LoadAsync(() =>
            {
                var controller = new GameOverScreenController(GameConfig.GameplayInfo, gameEndResults, AssetLoadService);
                controller.ReplayRequested += OnReplayRequested;
                controller.MainMenuRequested += OnMainMenuRequested;
                return controller;
            });
        }

        private void UnloadGameOverScreen()
        {
            if (GameOverScreenController == null) return;
            GameOverScreenController.ReplayRequested -= OnReplayRequested;
            GameOverScreenController.MainMenuRequested -= OnMainMenuRequested;
            ControllerLoadService.Unload(GameOverScreenController);
        }
        
        private async UniTask<ILoadingScreenController> LoadLoadingScreenAsync()
        {
            return await ControllerLoadService.LoadAsync(() =>
            {
                var controller = new LoadingScreenScreenController(AssetLoadService, GameConfig.LoadingScreenViewKey, 
                    GameConfig.LoadingScreenTransitionDuration);
                return controller;
            });
        }

        private void UnloadLoadingScreen()
        {
            if (LoadingScreenController == null) return;
            ControllerLoadService.Unload(LoadingScreenController);
        }

        private async void OnPlayRequested()
        {
            await LoadingScreenController.Show();
            
            UnloadMainMenu();
            GameplayController = await LoadGameplayAsync();
            AudioController.RegisterGameplayEvents(GameplayController);
            
            await LoadingScreenController.Hide();

            GameplayController.GameEnded += OnGameEnded;
            GameplayController.StartGame();
        }

        private async void OnGameEnded(GameEndResults gameEndResults)
        {
            GameplayController.GameEnded -= OnGameEnded;
            GameOverScreenController = await LoadGameOverScreenAsync(gameEndResults);
            if (gameEndResults.TriggeredEndRules == null)
            {
                Debug.Log($"[GameplayController] Game Ended with no end rule triggers");
                return;
            }
            
            var allMessages = string.Join("; ", gameEndResults.TriggeredEndRules.Select(r => r.Message).ToArray());
            Debug.Log($"[GameplayController] Game Ended: {allMessages}");
        }
        
        private async void OnReplayRequested()
        {
            await LoadingScreenController.Show();
            UnloadGameOverScreen();
            UnloadGameplay();
            OnPlayRequested();
        }

        private async void OnMainMenuRequested()
        {
            await LoadingScreenController.Show();
            UnloadGameOverScreen();
            UnloadGameplay();
            await ShowMainMenuAsync();
        }
        
        private async UniTask ShowMainMenuAsync()
        {
            await LoadingScreenController.Show();
            MainMenuController = await ControllerLoadService.LoadAsync(() =>
            {
                var controller = new MainMenuController(AssetLoadService, GameConfig.MainMenuViewKey);
                controller.PlayRequested += OnPlayRequested;
                controller.ExitRequested += OnExitRequested;
                return controller;
            });
            await LoadingScreenController.Hide();
        }
        
        private void UnloadMainMenu()
        {
            if (MainMenuController == null) return;
            MainMenuController.PlayRequested -= OnPlayRequested;
            MainMenuController.ExitRequested -= OnExitRequested;
            ControllerLoadService.Unload(MainMenuController);
        }

        private static void OnExitRequested()
        {
            Application.Quit();
        }
    }
}