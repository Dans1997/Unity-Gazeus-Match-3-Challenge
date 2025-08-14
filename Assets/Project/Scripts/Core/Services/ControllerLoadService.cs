using System;
using Cysharp.Threading.Tasks;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Script.Core.Services
{
    public class ControllerLoadService : IControllerLoadService
    {
        public async UniTask<T> LoadAsync<T>(Func<T> createController) where T : IController
        {
            try
            {
                var controller = createController();
                await controller.Initialize();
                return controller;
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception when initializing controller: {e}");
                throw;
            }
        }

        public void Unload(IController controller)
        {
            controller?.Dispose();
        }
    }
}