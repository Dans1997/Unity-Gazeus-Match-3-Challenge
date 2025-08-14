using System;
using Cysharp.Threading.Tasks;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Script.Core.Addressables
{
    public class AddressablesAssetProvider : IAssetProvider
    {
        public async UniTask<T> LoadAssetAsync<T>(string key)
        {
            try
            {
                var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(key);
                await handle;

                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    return handle.Result;
                }

                Debug.LogError($"Failed to load asset with key: {key}");
                throw new Exception($"Failed to load asset with key: {key}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception while loading asset with key {key}: {e}");
                throw;
            }
        }

        public UniTask<T> LoadAssetAsync<T>(Enum key)
        {
            return LoadAssetAsync<T>(key.ToString());
        }

        public async UniTask<T> InstantiateAsync<T>(string key, Transform parent = null)
        {
            try
            {
                var handle = UnityEngine.AddressableAssets.Addressables.InstantiateAsync(key, parent);
                await handle;
                return handle.Result.GetComponentInChildren<T>();
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception while instantiating asset with key {key}: {e}");
                throw;
            }
        }

        public UniTask<T> InstantiateAsync<T>(Enum key, Transform parent = null)
        {
            return InstantiateAsync<T>(key.ToString(), parent);
        }

        public void Release<T>(T asset)
        {
            if (asset == null) return;
            
            try
            {
                UnityEngine.AddressableAssets.Addressables.Release(asset);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to release asset: {ex}");
            }
        }
    }
}