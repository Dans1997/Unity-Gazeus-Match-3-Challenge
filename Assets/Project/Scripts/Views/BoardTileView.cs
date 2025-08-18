using Gazeus.DesafioMatch3.Models.BoardTiles;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles;
using Lean.Pool;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    public class BoardTileView : SerializedMonoBehaviour, IBoardTileView
    {
        public Transform Transform => transform;
        
        [OdinSerialize, ReadOnly] private BoardTileLoadedAssets loadedAssets;
        [OdinSerialize, ReadOnly] private Image tileIconImage;

        private void Awake()
        {
            tileIconImage = GetComponentInChildren<Image>();
        }

        public void ConfigureTileVisuals(BoardTileLoadedAssets loadedAssets)
        {
            this.loadedAssets = loadedAssets;
            tileIconImage.sprite = loadedAssets.TileSprite;
        }

        public void PlayDestructionSequence()
        {
            // TODO: 
        }
    }
}