using Cysharp.Threading.Tasks;
using Lean.Pool;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Helpers
{
    public static class LeanPoolHelper
    {
        public static void DespawnAfterDelay(this GameObject gameObject, float delay)
        {
            DespawnRoutine(gameObject, delay).Forget();
        }

        private static async UniTaskVoid DespawnRoutine(GameObject gameObject, float delay)
        {
            if (gameObject == null) return;

            await UniTask.Delay
            (
                System.TimeSpan.FromSeconds(delay)
            );

            if (gameObject != null && gameObject.activeInHierarchy)
            {
                LeanPool.Despawn(gameObject);
            }
        }
    }
}