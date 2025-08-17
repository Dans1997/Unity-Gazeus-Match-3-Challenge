using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Helpers
{
    public static class TransformHelpers
    {
        public static void DestroyAllChildren(this Transform parent, bool immediate = false)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i).gameObject;

#if UNITY_EDITOR
                if (immediate && !Application.isPlaying)
                    Object.DestroyImmediate(child);
                else
#endif
                    Object.Destroy(child);
            }
        }
    }
}