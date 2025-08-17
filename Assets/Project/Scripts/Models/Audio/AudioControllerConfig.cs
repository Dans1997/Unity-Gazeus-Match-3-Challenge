using System;
using System.Collections.Generic;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Sirenix.Serialization;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct AudioControllerConfig
    {
        [OdinSerialize] public Dictionary<AudioKey, AudioClipConfig> AudioClips { get; private set; }
        [OdinSerialize] public AudioSourceKey MusicSourceKey { get; private set; }
        [OdinSerialize] public AudioSourceKey SfxSourceKey { get; private set; }
    }
}