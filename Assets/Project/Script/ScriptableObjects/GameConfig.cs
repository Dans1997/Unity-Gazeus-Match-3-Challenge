using Gazeus.DesafioMatch3.Models;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig")]
    public class GameConfig : SerializedScriptableObject
    {
        [OdinSerialize] public SceneLoadInfo MainMenuSceneLoadInfo { get; private set; }
        [OdinSerialize] public SceneLoadInfo GameplaySceneLoadInfo { get; private set; }
    }
}