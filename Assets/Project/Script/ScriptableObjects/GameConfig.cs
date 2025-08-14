using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.Project.Script.Enums;
using Gazeus.Match3Challenge.Project.Script.Interfaces.Configs;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig")]
    public class GameConfig : SerializedScriptableObject, IGameConfig
    {
        [FoldoutGroup("Main Menu")] [OdinSerialize] public MainMenuViewKey MainMenuViewKey { get; private set; }
        
        [FoldoutGroup("Loading Screen")] [OdinSerialize] public LoadingScreenViewKey LoadingScreenViewKey { get; private set; }
        [FoldoutGroup("Loading Screen")] [OdinSerialize] public float LoadingScreenTransitionDuration { get; private set; }
        
        [FoldoutGroup("Gameplay")] [OdinSerialize] public GameplayInfo GameplayInfo { get; private set; }
    }
}