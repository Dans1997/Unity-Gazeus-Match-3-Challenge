using Cysharp.Threading.Tasks;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.DesafioMatch3.Views;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class LoadingScreenScreenController : ILoadingScreenController
    {
        public IAssetProvider AssetProvider { get; private set; }
        private readonly LoadingScreenViewKey loadingScreenViewKey;
        private readonly float transitionDuration;
        private LoadingScreenView loadingScreenView;

        public LoadingScreenScreenController(IAssetProvider assetProvider, LoadingScreenViewKey loadingScreenViewKey,
            float transitionDuration)
        {
            AssetProvider = assetProvider;
            this.loadingScreenViewKey = loadingScreenViewKey;
            this.transitionDuration = transitionDuration;
        }
        
        public async UniTask Initialize()
        {
            loadingScreenView = await AssetProvider.InstantiateAsync<LoadingScreenView>(loadingScreenViewKey);
        }

        public void Dispose()
        {
            if (loadingScreenView == null) return;
            AssetProvider.Release(loadingScreenView.gameObject);
            loadingScreenView = null;
        }

        public UniTask Show()
        {
            return loadingScreenView == null 
                ? UniTask.CompletedTask 
                : FadeCanvasGroupAsync(loadingScreenView.CanvasGroup, 1f, transitionDuration);
        }

        public UniTask Hide()
        {
            return loadingScreenView == null 
                ? UniTask.CompletedTask 
                : FadeCanvasGroupAsync(loadingScreenView.CanvasGroup, 0f, transitionDuration);
        }

        private static async UniTask FadeCanvasGroupAsync(CanvasGroup canvasGroup, float targetAlpha, float duration)
        {
            var startAlpha = canvasGroup.alpha;
            var time = 0f;
            
            if (targetAlpha > 0) 
            {
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            }

            while (time < duration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
                await UniTask.Yield();
            }

            canvasGroup.alpha = targetAlpha;
            
            if (targetAlpha == 0)
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }
        }
    }
}