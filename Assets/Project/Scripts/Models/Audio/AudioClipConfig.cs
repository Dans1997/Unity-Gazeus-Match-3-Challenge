using System;
using Sirenix.Serialization;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct AudioClipConfig
    {
        [OdinSerialize] public float Volume { get; private set; }
        [OdinSerialize] public bool Loop { get; private set; }

        public AudioClipConfig(float volume = 1f, bool loop = false)
        {
            Volume = volume;
            Loop = loop;
        }
    }
}