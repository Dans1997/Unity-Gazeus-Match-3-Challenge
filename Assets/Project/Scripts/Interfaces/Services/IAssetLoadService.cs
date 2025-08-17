using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables
{
    public interface IAssetLoadService
    {
        UniTask<T> LoadAssetAsync<T>(string key);
        UniTask<T> LoadAssetAsync<T>(Enum key);
        UniTask<T[]> LoadAssetsAsync<TEnum, T>(TEnum[] keys);
        UniTask<T> InstantiateAsync<T>(Enum key, Transform parent = null);
        void Release<T>(T asset);
    }
}