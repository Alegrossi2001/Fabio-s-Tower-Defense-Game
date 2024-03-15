using UnityEditor;

namespace Dypsloom.RhythmTimeline.Editor
{
    using Dypsloom.RhythmTimeline.Core.Playables;
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(RhythmClip), true)]
    [CanEditMultipleObjects]
    public class RhythmClipInspector : Editor
    { }
}
