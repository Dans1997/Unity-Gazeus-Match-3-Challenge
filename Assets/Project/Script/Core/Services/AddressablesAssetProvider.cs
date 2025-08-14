using Cysharp.Threading.Tasks;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Script.Core.Addressables
{
    public class AddressablesAssetProvider : IAssetProvider
    {
        public async UniTask<T> LoadAssetAsync<T>(string key)
        {
            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(key);
            await handle;
            return handle.Result;
        }

        public async UniTask<T> InstantiateAsync<T>(string key, Transform parent = null)
        {
            var handle = UnityEngine.AddressableAssets.Addressables.InstantiateAsync(key, parent);
            await handle;
            return handle.Result.GetComponent<T>();
        }

        public void Release<T>(T asset)
        {
            UnityEngine.AddressableAssets.Addressables.Release(asset);
        }
    }
}