using System;
using DG.Tweening;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    public class BoardCellView : SerializedMonoBehaviour, IBoardCellView
    {
        public event Action<IBoardCellView> Clicked;

        [OdinSerialize, ReadOnly] public Transform Transform => transform;
        [OdinSerialize, ReadOnly] public Vector2Int Position { get; private set; }
        [OdinSerialize, ReadOnly] public Button Button { get; private set; }
        [OdinSerialize, ReadOnly] public Image SelectionImage { get; private set; }

        private void Awake()
        {
            Button = GetComponentInChildren<Button>();
            SelectionImage = GetComponentInChildren<Image>();
        }

        private void Start()
        {
            Button.onClick.AddListener(OnTileClick);
            SetSelected(false);
        }
        
        private void OnDestroy() => Button.onClick.RemoveListener(OnTileClick);
        
        public void SetPosition(Vector2Int position) => Position = position;
        public void SetSelected(bool selected) => SelectionImage.enabled = selected;

        public void SetTile(Transform tileTransform)
        {
            tileTransform.SetParent(transform, false);
            tileTransform.position = transform.position;
        }
        
        public Tween SetTileAnimated(Transform tileTransform)
        {
            tileTransform.SetParent(transform);
            tileTransform.DOKill();

            return tileTransform.DOMove(transform.position, 0.5f);
        }

        private void OnTileClick() => Clicked?.Invoke(this);
    }
}
