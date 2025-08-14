using System;
using Cysharp.Threading.Tasks;
using Gazeus.DesafioMatch3.Controllers;
using Gazeus.DesafioMatch3.Project.Script.Core.Services;
using Gazeus.DesafioMatch3.ScriptableObjects;
using Gazeus.DesafioMatch3.Views;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gazeus.DesafioMatch3.Core.GameInitialization
{
    public class GameInitializationContainer : SerializedMonoBehaviour
    {
        [Header("Config")]
        [OdinSerialize] private GameConfig gameConfig;

        [Header("Debug")] 
        [OdinSerialize, ReadOnly] private SceneLoadService _mainMenuSceneLoadService;
        [OdinSerialize, ReadOnly] private MainMenuController mainMenuController;
        [OdinSerialize, ReadOnly] private SceneLoadService _gameplaySceneLoadService;
        [OdinSerialize, ReadOnly] private GameplayController gameplayController;

        private async void Start()
        {
            try
            {
                await Addressables.InitializeAsync().ToUniTask();
                _mainMenuSceneLoadService = new SceneLoadService(this, gameConfig.MainMenuSceneLoadInfo);
                _gameplaySceneLoadService = new SceneLoadService(this, gameConfig.GameplaySceneLoadInfo);
                mainMenuController = await LoadMainMenuScene();
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception while initializing game: {e}");
                throw;
            }
        }

        private void OnDestroy()
        {
            if (mainMenuController != null)
            {
                mainMenuController.PlayRequested -= OnPlayRequested;
                mainMenuController.ExitRequested -= OnExitRequested;
                mainMenuController.Dispose();
            }
        }

        private async UniTask<MainMenuController> LoadMainMenuScene()
        {
            await _mainMenuSceneLoadService.LoadSceneAsync();

            var controller = new MainMenuController(FindObjectOfType<MainMenuView>()); // TODO: Replace FindObjectOfType with parameter injection if feasible
            controller.PlayRequested += OnPlayRequested;
            controller.ExitRequested += OnExitRequested;
            return controller;
        }

        private async UniTask UnloadMainMenuScene()
        {
            await _mainMenuSceneLoadService.UnloadSceneAsync();
        }

        private async UniTask<GameplayController> LoadGameplayScene()
        {
            await _gameplaySceneLoadService.LoadSceneAsync();

            var controller = FindObjectOfType<GameplayController>(); // TODO: Replace FindObjectOfType with parameter injection if feasible
            return controller;
        }

        private async void OnPlayRequested()
        {
            await UnloadMainMenuScene();
            gameplayController = await LoadGameplayScene();
            // TODO
        }
        
        private static void OnExitRequested()
        {
            Application.Quit();
        }
    }
}