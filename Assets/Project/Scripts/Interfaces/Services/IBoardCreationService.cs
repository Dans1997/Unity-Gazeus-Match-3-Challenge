using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface IBoardCreationService
    {
        public IBoardState CreateBoard();
    }
}