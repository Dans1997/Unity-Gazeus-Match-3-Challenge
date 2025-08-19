using System;
using Cysharp.Threading.Tasks;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers
{
    public interface IGameFlowController : IDisposable
    {
        UniTask StartGameAsync();
    }
}