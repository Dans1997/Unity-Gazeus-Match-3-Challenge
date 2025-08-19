using Gazeus.DesafioMatch3.Models.BoardTiles;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles
{
    public interface IBoardTileView
    {
        Transform Transform { get; }
        void ConfigureTileVisuals(BoardTileLoadedAssets assets);
        GameObject PlayDestructionSequence();
    }
}