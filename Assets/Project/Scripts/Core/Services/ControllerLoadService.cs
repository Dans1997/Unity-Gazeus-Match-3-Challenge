using System;
using Cysharp.Threading.Tasks;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Services;

namespace Gazeus.Match3Challenge.Project.Script.Core.Services
{
    public class ControllerLoadService : IControllerLoadService
    {
        public async UniTask<T> LoadAsync<T>(Func<T> createController) where T : IController
        {
            var controller = createController();
            await controller.Initialize();
            return controller;
        }

        public void Unload(IController controller)
        {
            controller?.Dispose();
        }
    }
}