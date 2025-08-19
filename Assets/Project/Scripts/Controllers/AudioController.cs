using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Addressables;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Controllers;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class AudioController : IAudioController
    {
        public AudioControllerConfig AudioControllerConfig { get; private set; }
        public IAssetLoadService AssetLoadService { get; private set; }
        public AudioClip[] LoadedAudioClips { get; private set; }
        public AudioSource MusicSource { get; private set; }
        public AudioSource SfxSource { get; private set; }
        
        private IGameplayController gameplayController;

        public AudioController(AudioControllerConfig audioControllerConfig, IAssetLoadService assetLoadService)
        {
            AudioControllerConfig = audioControllerConfig;
            AssetLoadService = assetLoadService;
        }
        
        public async UniTask Initialize()
        {
            LoadedAudioClips = await AssetLoadService.LoadAssetsAsync<AudioKey, AudioClip>(AudioControllerConfig.AudioClips.Keys.ToArray());
            MusicSource = await AssetLoadService.InstantiateAsync<AudioSource>(AudioControllerConfig.MusicSourceKey);
            SfxSource = await AssetLoadService.InstantiateAsync<AudioSource>(AudioControllerConfig.SfxSourceKey);

            PlayMusic(AudioKey.MysticalEnergySoundtrack);
        }
        
        public void Dispose()
        {
            try
            {
                UnregisterGameplayController();

                if (MusicSource != null)
                {
                    MusicSource.Stop();
                    AssetLoadService?.Release(MusicSource.gameObject);
                    MusicSource = null;
                }

                if (SfxSource != null)
                {
                    SfxSource.Stop();
                    AssetLoadService?.Release(SfxSource.gameObject);
                    SfxSource = null;
                }

                if (LoadedAudioClips != null && AssetLoadService != null)
                {
                    foreach (var clip in LoadedAudioClips)
                    {
                        if (clip != null) AssetLoadService.Release(clip);
                    }
                }
                
                LoadedAudioClips = null;
                gameplayController = null;
            }
            catch (Exception e)
            {
                Debug.LogError($"[AudioController] Dispose failed: {e}");
            }
        }

        public void PlayMusic(AudioKey audioKey)
        {
            if (!AudioControllerConfig.AudioClips.TryGetValue(audioKey, out var config))
            {
                Debug.LogWarning($"Music {audioKey} not available");
                return;
            }

            MusicSource.clip = LoadedAudioClips[(int)audioKey];
            MusicSource.volume = config.Volume;
            MusicSource.loop = config.Loop;
            MusicSource.Play();
        }
        
        public void PlaySfx(AudioKey audioKey)
        {
            if (!AudioControllerConfig.AudioClips.TryGetValue(audioKey, out var config))
            {
                Debug.LogWarning($"Music {audioKey} not available");
                return;
            }

            SfxSource.PlayOneShot(LoadedAudioClips[(int)audioKey], config.Volume);
        }

        public void RegisterGameplayEvents(IGameplayController controller)
        {
            gameplayController = controller;
            
            controller.GameStarted += OnGameStarted;
            controller.TileClicked += OnTileClicked;
            controller.TileSelected += OnTileSelected;
            controller.TileSwapped += OnTileSwapped;
            controller.ScoreUpdated += OnScoreUpdated;
            controller.GameEnded += OnGameEnded;
        }

        private void UnregisterGameplayController()
        {
            if (gameplayController == null) return;
            
            gameplayController.GameStarted -= OnGameStarted;
            gameplayController.TileClicked -= OnTileClicked;
            gameplayController.TileSelected -= OnTileSelected;
            gameplayController.TileSwapped -= OnTileSwapped;
            gameplayController.ScoreUpdated -= OnScoreUpdated;
            gameplayController.GameEnded -= OnGameEnded;

            gameplayController = null;
        }

        private void OnGameStarted() => PlaySfx(AudioKey.GameStartedSfx);
        private void OnTileClicked() => PlaySfx(AudioKey.TileClickedSfx);
        private void OnTileSelected() => PlaySfx(AudioKey.TileSelectedSfx);
        private void OnTileSwapped() => PlaySfx(AudioKey.TileSwappedSfx);
        private void OnScoreUpdated() => PlaySfx(AudioKey.ScoreUpdatedSfx);

        private void OnGameEnded(GameEndResults gameEndResults)
        {
            gameplayController.GameEnded -= OnGameEnded;
            PlaySfx(AudioKey.GameEndedSfx);
        }
    }
}