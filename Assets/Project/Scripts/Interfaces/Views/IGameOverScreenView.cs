using System;
using Gazeus.DesafioMatch3.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles
{
    public interface IGameOverScreenView : IView
    {
        event Action ReplayClicked;
        event Action MainMenuClicked;
        
        void SetGameEndResults(GameplayInfo gameplayInfo, GameEndResults gameEndResults);
    }
}