using System;
using Gazeus.DesafioMatch3.Models;
using Gazeus.Match3Challenge.Project.Scripts.Helpers;
using Gazeus.Match3Challenge.Project.Scripts.Interfaces.Tiles;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    public class GameOverScreenView : SerializedMonoBehaviour, IGameOverScreenView
    {
        public event Action ReplayClicked;
        public event Action MainMenuClicked;
        public Transform Transform => transform;
        
        [OdinSerialize] public TMP_Text FinalScoreText { get; private set; }
        [OdinSerialize] public TMP_Text FinalTimeText { get; private set; }
        [OdinSerialize] public Button ReplayButton { get; private set; }
        [OdinSerialize] public Button BackToMainMenuButton { get; private set; }

        private void Awake()
        {
            ReplayButton.onClick.AddListener(OnReplayButtonClicked);
            BackToMainMenuButton.onClick.AddListener(OnBackToMainMenuButtonClicked);
        }

        private void OnDestroy()
        {
            ReplayButton.onClick.RemoveAllListeners();
            BackToMainMenuButton.onClick.RemoveAllListeners();
        }

        public void SetGameEndResults(GameplayInfo gameplayInfo, GameEndResults gameEndResults)
        {
            FinalScoreText.text = gameEndResults.FinalScore.ToString();
            FinalTimeText.text = gameEndResults.FinalTimeInSeconds.FormatTime();
        }

        private void OnReplayButtonClicked() => ReplayClicked?.Invoke();
        private void OnBackToMainMenuButtonClicked() => MainMenuClicked?.Invoke();
    }
}