using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    public class TileSpotView : SerializedMonoBehaviour
    {
        public event Action<int, int> Clicked;

        [OdinSerialize, ReadOnly] private Button _button;
        [OdinSerialize, ReadOnly] private int _x;
        [OdinSerialize, ReadOnly] private int _y;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _button.onClick.AddListener(OnTileClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnTileClick);
        }

        public Tween AnimatedSetTile(GameObject tile)
        {
            tile.transform.SetParent(transform);
            tile.transform.DOKill();

            return tile.transform.DOMove(transform.position, 0.3f);
        }

        public void SetPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public void SetTile(GameObject tile)
        {
            tile.transform.SetParent(transform, false);
            tile.transform.position = transform.position;
        }

        private void OnTileClick()
        {
            Clicked?.Invoke(_x, _y);
        }
    }
}
