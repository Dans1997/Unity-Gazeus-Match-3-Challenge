using System;
using Cysharp.Threading.Tasks;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Gazeus.DesafioMatch3.Project.Script.Core.Services
{
    public class SceneLoadService
    {
        private readonly MonoBehaviour coroutineRunner;
        private readonly SceneLoadInfo sceneLoadInfo;
        private SceneInstance loadedSceneInstance;
        
        private SceneKey LoadingScreenKey => sceneLoadInfo.LoadingScreenKey;
        
        public SceneLoadService(MonoBehaviour coroutineRunner, SceneLoadInfo sceneLoadInfo)
        {
            this.coroutineRunner = coroutineRunner;
            this.sceneLoadInfo = sceneLoadInfo;
        }

        public async UniTask LoadSceneAsync()
        {
            if (loadedSceneInstance.Scene.isLoaded)
            {
                Debug.LogWarning($"Scene {sceneLoadInfo.SceneKey} is already loaded");
                return;
            }
            
            SceneInstance loadingScreenInstance = default;
            
            if (sceneLoadInfo.UseLoadingScreen)
            {
                var loadParameters = new LoadSceneParameters
                {
                    loadSceneMode = LoadSceneMode.Additive
                };

                loadingScreenInstance = await LoadSingleScene(LoadingScreenKey, loadParameters);
                await UniTask.Delay(TimeSpan.FromSeconds(3f));
            }

            loadedSceneInstance = await LoadSingleScene(sceneLoadInfo.SceneKey, sceneLoadInfo.LoadSceneParameters);

            if (sceneLoadInfo.UseLoadingScreen)
            {
                await UnloadSingleScene(loadingScreenInstance);
            }
        }

        public async UniTask UnloadSceneAsync()
        {
            if (!loadedSceneInstance.Scene.isLoaded)
            {
                Debug.LogWarning($"Scene {sceneLoadInfo.SceneKey} is already unloaded.");
                return;
            }
            
            await UnloadSingleScene(loadedSceneInstance);
        }

        private async UniTask<SceneInstance> LoadSingleScene(SceneKey key, LoadSceneParameters loadSceneParameters)
        {
            try
            {
                var handle = Addressables.LoadSceneAsync(key.ToString(), loadSceneParameters);
                await handle.ToUniTask(coroutineRunner);

                Debug.Log($"Successfully loaded scene: {key}");
                return handle.Result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception while loading scene {key}: {ex}");
                throw;
            }
        }

        private async UniTask UnloadSingleScene(SceneInstance sceneInstance)
        {
            try
            {
                var sceneName = sceneInstance.Scene.name;
                var handle = Addressables.UnloadSceneAsync(sceneInstance);
                await handle.ToUniTask(coroutineRunner);

                Debug.Log($"Successfully unloaded scene: {sceneName}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception while unloading scene {sceneInstance.Scene.name}: {ex}");
                throw;
            }
        }

    }
}