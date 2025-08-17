using System;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers
{
    public interface IGameOverScreenController : IController
    {
        event Action ReplayRequested;
        event Action MainMenuRequested;
    }
}