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
        public event Action<Vector2Int> Clicked;
        public Transform Transform => transform;
        
        [OdinSerialize, ReadOnly] private Button _button;
        [OdinSerialize, ReadOnly] private Vector2Int _position;
        
        private void Awake() => _button = GetComponent<Button>();
        private void Start() => _button.onClick.AddListener(OnTileClick);
        private void OnDestroy() => _button.onClick.RemoveListener(OnTileClick);

        public Tween AnimatedSetTile(Transform tileTransform)
        {
            tileTransform.SetParent(transform);
            tileTransform.DOKill();

            return tileTransform.DOMove(transform.position, 0.3f);
        }
        
        public void SetPosition(Vector2Int position) => _position = position;

        public void SetTile(Transform tileTransform)
        {
            tileTransform.SetParent(transform, false);
            tileTransform.position = transform.position;
        }

        private void OnTileClick() => Clicked?.Invoke(_position);
    }
}
