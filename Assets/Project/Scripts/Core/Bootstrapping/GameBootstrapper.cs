using System;
using Cysharp.Threading.Tasks;
using Gazeus.DesafioMatch3.Core.GameInitialization;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Script.Core.Addressables;
using Gazeus.Match3Challenge.Project.Script.Core.Services;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Configs;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gazeus.Match3Challenge.Project.Scripts.Core.Bootstrapping
{
    public class GameBootstrapper : SerializedMonoBehaviour
    {
        [OdinSerialize, ReadOnly] public IGameFlowController GameFlowController { get; private set; }

        private async void Start()
        {
            try
            {
                await Addressables.InitializeAsync().ToUniTask();
            
                var assetService = new AddressablesAssetLoadService();
                var controllerLoader = new ControllerLoadService();
                var gameConfig = await assetService.LoadAssetAsync<IGameConfig>(AddressablesAssetKeys.GameConfigKey);
        
                GameFlowController = new GameFlowController(gameConfig, assetService, controllerLoader);
                await GameFlowController.StartGameAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameBoostrapper] Exception while boostrapping game: {e}");
                throw;
            }
        }

        private void OnDestroy()
        {
            GameFlowController?.Dispose();
        }
    }
}