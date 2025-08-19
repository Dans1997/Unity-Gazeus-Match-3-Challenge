using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Models;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Services
{
    public interface ITileGenerationService
    {
        public void GenerateNextTile(TileInfo tileInfo, IBoardState boardState);
    }
}