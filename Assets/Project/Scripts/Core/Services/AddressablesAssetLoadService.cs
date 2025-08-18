using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Script.Core.Addressables
{
    public class AddressablesAssetLoadService : IAssetLoadService
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
                
                throw new Exception($"Failed to load asset with key: {key}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception while loading asset with key {key}: {e.Message}");
                throw;
            }
        }

        public async UniTask<T[]> LoadAssetsAsync<T>(string[] keys)
        {
            if (keys == null || keys.Length == 0)
            {
                throw new ArgumentException("Keys array cannot be null or empty", nameof(keys));
            }

            var tasks = new UniTask<T>[keys.Length];

            for (var i = 0; i < keys.Length; i++)
            {
                tasks[i] = LoadAssetAsync<T>(keys[i]);
            }

            return await UniTask.WhenAll(tasks);
        }

        public UniTask<T[]> LoadAssetsAsync<TEnum, T>(TEnum[] keys)
        {
            return LoadAssetsAsync<T>(keys.Select(k => k.ToString()).ToArray());
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
                Debug.LogError($"Exception while instantiating asset with key {key}: {e.Message}");
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
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to release asset: {e.Message}");
            }
        }
    }
}