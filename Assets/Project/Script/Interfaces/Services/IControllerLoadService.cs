using System;
using Cysharp.Threading.Tasks;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Services
{
    public interface IControllerLoadService
    {
        UniTask<T> LoadAsync<T>(Func<T> createController) where T : IController;
        void Unload(IController controller);
    }
}