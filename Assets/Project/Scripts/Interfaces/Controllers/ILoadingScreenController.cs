using Cysharp.Threading.Tasks;

namespace Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers
{
    public interface ILoadingScreenController : IController
    {
        UniTask Show();
        UniTask Hide();
    }
}