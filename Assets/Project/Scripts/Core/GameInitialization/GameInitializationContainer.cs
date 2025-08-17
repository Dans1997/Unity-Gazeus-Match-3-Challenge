using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gazeus.DesafioMatch3.Controllers;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Script.Core.Addressables;
using Gazeus.Match3Challenge.Project.Script.Core.Services;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Configs;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gazeus.DesafioMatch3.Core.GameInitialization
{
    public class GameInitializationContainer : SerializedMonoBehaviour
    {
        [Header("Configs")] 
        [OdinSerialize, ReadOnly] private IGameConfig gameConfig;
        
        [Header("Services")] 
        [OdinSerialize, ReadOnly] private IAssetLoadService _assetLoadService;
        [OdinSerialize, ReadOnly] private IControllerLoadService controllerLoadService;
        
        [Header("Controllers")] 
        [OdinSerialize, ReadOnly] private IAudioController audioController;
        [OdinSerialize, ReadOnly] private IMainMenuController mainMenuController;
        [OdinSerialize, ReadOnly] private IGameplayController gameplayController;
        [OdinSerialize, ReadOnly] private IGameOverScreenController gameOverScreenController;
        [OdinSerialize, ReadOnly] private ILoadingScreenController loadingScreenController;
        
        private async void Start()
        {
            try
            {
                await Addressables.InitializeAsync().ToUniTask();
                
                _assetLoadService = new AddressablesAssetLoadService();
                controllerLoadService = new ControllerLoadService();
                
                gameConfig = await _assetLoadService.LoadAssetAsync<IGameConfig>(AddressablesAssetKeys.GameConfigKey);

                audioController = await controllerLoadService.LoadAsync(() =>
                {
                    var controller = new AudioController(gameConfig.AudioControllerConfig, _assetLoadService);
                    return controller;
                });
                
                loadingScreenController = await LoadLoadingScreenAsync();
                await ShowMainMenuAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception while initializing game: {e}");
                throw;
            }
        }

        private void OnDestroy()
        {
            UnloadMainMenu();
            UnloadGameplay();
            UnloadLoadingScreen();
        }

        private async UniTask<IGameplayController> LoadGameplayAsync()
        {
            return await controllerLoadService.LoadAsync(() =>
            {
                var controller = new GameplayController(gameConfig.GameplayInfo, _assetLoadService);
                return controller;
            });
        }

        private void UnloadGameplay()
        {
            if (gameplayController == null) return;
            controllerLoadService.Unload(gameplayController);
        }

        private async UniTask<IGameOverScreenController> LoadGameOverScreenAsync(GameEndResults gameEndResults)
        {
            return await controllerLoadService.LoadAsync(() =>
            {
                var controller = new GameOverScreenController(gameConfig.GameplayInfo, gameEndResults, _assetLoadService);
                controller.ReplayRequested += OnReplayRequested;
                controller.MainMenuRequested += OnMainMenuRequested;
                return controller;
            });
        }

        private void UnloadGameOverScreen()
        {
            if (gameOverScreenController == null) return;
            gameOverScreenController.ReplayRequested -= OnReplayRequested;
            gameOverScreenController.MainMenuRequested -= OnMainMenuRequested;
            controllerLoadService.Unload(gameOverScreenController);
        }
        
        private async UniTask<ILoadingScreenController> LoadLoadingScreenAsync()
        {
            return await controllerLoadService.LoadAsync(() =>
            {
                var controller = new LoadingScreenScreenController(_assetLoadService, gameConfig.LoadingScreenViewKey, 
                    gameConfig.LoadingScreenTransitionDuration);
                return controller;
            });
        }

        private void UnloadLoadingScreen()
        {
            if (loadingScreenController == null) return;
            controllerLoadService.Unload(loadingScreenController);
        }

        private async void OnPlayRequested()
        {
            await loadingScreenController.Show();
            
            UnloadMainMenu();
            gameplayController = await LoadGameplayAsync();
            audioController.RegisterGameplayEvents(gameplayController);
            
            await loadingScreenController.Hide();

            gameplayController.GameEnded += OnGameEnded;
            gameplayController.StartGame();
        }

        private async void OnGameEnded(GameEndResults gameEndResults)
        {
            gameplayController.GameEnded -= OnGameEnded;
            gameOverScreenController = await LoadGameOverScreenAsync(gameEndResults);
            
            var allMessages = string.Join("; ", gameEndResults.TriggeredEndRules.Select(r => r.Message).ToArray());
            Debug.Log($"[GameplayController] Game Ended: {allMessages}");
        }
        
        private async void OnReplayRequested()
        {
            await loadingScreenController.Show();
            UnloadGameOverScreen();
            UnloadGameplay();
            OnPlayRequested();
        }

        private async void OnMainMenuRequested()
        {
            await loadingScreenController.Show();
            UnloadGameOverScreen();
            UnloadGameplay();
            await ShowMainMenuAsync();
        }
        
        private async UniTask ShowMainMenuAsync()
        {
            await loadingScreenController.Show();
            mainMenuController = await controllerLoadService.LoadAsync(() =>
            {
                var controller = new MainMenuController(_assetLoadService, gameConfig.MainMenuViewKey);
                controller.PlayRequested += OnPlayRequested;
                controller.ExitRequested += OnExitRequested;
                return controller;
            });
            await loadingScreenController.Hide();
        }
        
        private void UnloadMainMenu()
        {
            if (mainMenuController == null) return;
            mainMenuController.PlayRequested -= OnPlayRequested;
            mainMenuController.ExitRequested -= OnExitRequested;
            controllerLoadService.Unload(mainMenuController);
        }

        private static void OnExitRequested()
        {
            Application.Quit();
        }
    }
}