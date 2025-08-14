using System;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Sirenix.Serialization;
using UnityEngine.SceneManagement;

namespace Gazeus.DesafioMatch3.Models
{
    [Serializable]
    public struct SceneLoadInfo
    {
        [OdinSerialize] public SceneKey SceneKey { get; private set; }
        [OdinSerialize] public LoadSceneParameters LoadSceneParameters { get; private set; }
        [OdinSerialize] public bool UseLoadingScreen { get; private set; }
        [OdinSerialize] public SceneKey LoadingScreenKey { get; private set; }
    }
}