using Dypsloom.RhythmTimeline.Core;
using Dypsloom.RhythmTimeline.Core.Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dypsloom.RhythmTimeline.Editor
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(RhythmTimelineAsset), true)]
    public class RhythmTimelineAssetInspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            container.Add(new IMGUIContainer(OnInspectorGUI));
        
            var button = new Button();
            button.style.marginTop = 30;
            button.text = "Open In RhythmDirector";
            button.clicked += () =>
            {
#if UNITY_2021_3_OR_NEWER
                var director = FindFirstObjectByType<RhythmDirector>();
#else
                var director = FindObjectOfType<RhythmDirector>();
#endif
                
                if (director == null) {
                    Debug.LogWarning("The Rhythm Director could not be found in the scene.");
                    return;
                }
            
                director.PlayableDirector.playableAsset = target as RhythmTimelineAsset;
                director.RefreshBpm();
            
                Selection.SetActiveObjectWithContext(director.gameObject,director.gameObject);
            };
            
            container.Add(button);
            return container;
        }
    }
}