using System;
using Cysharp.Threading.Tasks;
using Gazeus.DesafioMatch3.Controllers;
using Gazeus.DesafioMatch3.Core.Services;
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
        [Header("Debug")] 
        [OdinSerialize, ReadOnly] private IGameConfig gameConfig;
        [OdinSerialize, ReadOnly] private IAssetProvider assetProvider;
        [OdinSerialize, ReadOnly] private IControllerLoadService controllerLoadService;
        [OdinSerialize, ReadOnly] private IMainMenuController mainMenuController;
        [OdinSerialize, ReadOnly] private IGameplayController gameplayController;
        [OdinSerialize, ReadOnly] private ILoadingScreenController loadingScreenController;
        
        private async void Start()
        {
            try
            {
                await Addressables.InitializeAsync().ToUniTask();
                assetProvider = new AddressablesAssetProvider();
                controllerLoadService = new ControllerLoadService();
                
                gameConfig = await assetProvider.LoadAssetAsync<IGameConfig>(AddressablesAssetKeys.GameConfigKey);
                loadingScreenController = await LoadLoadingScreenAsync();
                
                await loadingScreenController.Show();
                mainMenuController = await LoadMainMenuAsync();
                await loadingScreenController.Hide();
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

        private async UniTask<IMainMenuController> LoadMainMenuAsync()
        {
            return await controllerLoadService.LoadAsync(() =>
            {
                var controller = new MainMenuController(assetProvider, gameConfig.MainMenuViewKey);
                controller.PlayRequested += OnPlayRequested;
                controller.ExitRequested += OnExitRequested;
                return controller;
            });
        }

        private void UnloadMainMenu()
        {
            if (mainMenuController == null) return;
            mainMenuController.PlayRequested -= OnPlayRequested;
            mainMenuController.ExitRequested -= OnExitRequested;
            controllerLoadService.Unload(mainMenuController);
        }

        private async UniTask<IGameplayController> LoadGameplayAsync()
        {
            return await controllerLoadService.LoadAsync(() =>
            {
                var gameplayService = new GameplayService();
                var controller = new GameplayController(gameConfig.GameplayInfo, assetProvider, gameplayService);
                return controller;
            });
        }

        private void UnloadGameplay()
        {
            if (gameplayController == null) return;
            controllerLoadService.Unload(gameplayController);
        }
        
        private async UniTask<ILoadingScreenController> LoadLoadingScreenAsync()
        {
            return await controllerLoadService.LoadAsync(() =>
            {
                var controller = new LoadingScreenScreenController(assetProvider, gameConfig.LoadingScreenViewKey, 
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
            
            await loadingScreenController.Hide();
            
            // TODO
        }
        
        private static void OnExitRequested()
        {
            Application.Quit();
        }
    }
}