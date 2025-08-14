using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    public class MainMenuView : SerializedMonoBehaviour
    {
        [OdinSerialize] public Button PlayButton { get; private set; }
        [OdinSerialize] public Button ExitButton { get; private set; }

        public event Action PlayButtonClicked;
        public event Action ExitButtonClicked;

        private void Awake()
        {
            if (PlayButton != null) PlayButton.onClick.AddListener(() => PlayButtonClicked?.Invoke());
            if (ExitButton != null) ExitButton.onClick.AddListener(() => ExitButtonClicked?.Invoke());
        }

        private void OnDestroy()
        {
            if (PlayButton != null) PlayButton.onClick.RemoveAllListeners();
            if (ExitButton != null) ExitButton.onClick.RemoveAllListeners();
        }
    }
}