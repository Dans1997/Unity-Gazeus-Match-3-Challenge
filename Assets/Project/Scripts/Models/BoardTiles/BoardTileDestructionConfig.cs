using System;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Sirenix.Serialization;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct BoardTileDestructionConfig
    {
        [OdinSerialize] public float DestructionDuration { get; private set; }
        [OdinSerialize] public TileDestructionKey TileDestructionKey { get; private set; }
        [OdinSerialize] public AudioKey DestructionSoundEffectKey { get; private set; }
    }
}