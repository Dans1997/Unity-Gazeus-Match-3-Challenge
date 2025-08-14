using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Views
{
    public class LoadingScreenView : SerializedMonoBehaviour
    {
        [OdinSerialize, ReadOnly] public CanvasGroup CanvasGroup { get; private set; }
        
        private void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
        }
    }
}