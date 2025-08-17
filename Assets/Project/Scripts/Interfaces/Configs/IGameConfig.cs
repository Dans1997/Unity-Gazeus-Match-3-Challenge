using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Configs
{
    public interface IGameConfig
    {
        public MainMenuViewKey MainMenuViewKey { get; }
        public LoadingScreenViewKey LoadingScreenViewKey { get; }
        float LoadingScreenTransitionDuration { get; }
        public GameplayInfo GameplayInfo { get; }
        public AudioControllerConfig AudioControllerConfig { get; }
    }
}