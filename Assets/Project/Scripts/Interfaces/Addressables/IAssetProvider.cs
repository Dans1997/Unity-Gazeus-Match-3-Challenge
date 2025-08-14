using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables
{
    public interface IAssetProvider
    {
        UniTask<T> LoadAssetAsync<T>(string key);
        UniTask<T> LoadAssetAsync<T>(Enum key);
        UniTask<T> InstantiateAsync<T>(string key, Transform parent = null);
        UniTask<T> InstantiateAsync<T>(Enum key, Transform parent = null);
        void Release<T>(T asset);
    }
}