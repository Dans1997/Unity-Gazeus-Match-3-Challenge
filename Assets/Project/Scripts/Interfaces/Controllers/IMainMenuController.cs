using System;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers
{
    public interface IMainMenuController : IController
    {
        event Action PlayRequested;
        event Action ExitRequested;
    }
}