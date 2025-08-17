using Gazeus.DesafioMatch3.Project.Script.Enums;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers
{
    public interface IAudioController : IController
    {
        void PlayMusic(AudioKey config);
        void PlaySfx(AudioKey config);
        void RegisterGameplayEvents(IGameplayController controller);
    }
}