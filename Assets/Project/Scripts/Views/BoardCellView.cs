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
        
        [OdinSerialize, ReadOnly] private Button _button;
        [OdinSerialize, ReadOnly] private Image _image;

        private void Awake()
        {
            _button = GetComponentInChildren<Button>();
            _image = GetComponentInChildren<Image>();
        }

        private void Start()
        {
            _button.onClick.AddListener(OnTileClick);
            SetSelected(false);
        }
        
        private void OnDestroy() => _button.onClick.RemoveListener(OnTileClick);

        public Tween AnimatedSetTile(Transform tileTransform)
        {
            tileTransform.SetParent(transform);
            tileTransform.DOKill();

            return tileTransform.DOMove(transform.position, 0.5f);
        }
        
        public void SetPosition(Vector2Int position) => Position = position;
        public void SetSelected(bool selected) => _image.enabled = selected;

        public void SetTile(Transform tileTransform)
        {
            tileTransform.SetParent(transform, false);
            tileTransform.position = transform.position;
        }

        private void OnTileClick() => Clicked?.Invoke(this);
    }
}
