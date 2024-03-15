/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Playables
{
    using UnityEngine;
    using UnityEngine.Timeline;

    /// <summary>
    /// The tempo track has a custom inspector that lets you put markers at defined intervals.
    /// </summary>
    [TrackColor(0.1f, 0.4103774f, 7846687f)]
    public class TempoTrack : TrackAsset
    {
#if UNITY_EDITOR
        [Tooltip("The on beat editor settings.")]
        [SerializeField] protected TempoMarkerEditorSettings m_OnBeat;
        [Tooltip("The off beat editor settings.")]
        [SerializeField] protected TempoMarkerEditorSettings m_OffBeat;

        public TempoMarkerEditorSettings OnBeat => m_OnBeat;
        public TempoMarkerEditorSettings OffBeat => m_OffBeat;

        public TempoMarkerEditorSettings GetMarkerEditorSettings(TempoMarker tempoMarker)
        {
            if (m_OnBeat != null && tempoMarker.ID == m_OnBeat.ID) {
                return m_OnBeat;
            }

            if (m_OffBeat != null && tempoMarker.ID == m_OffBeat.ID) {
                return m_OffBeat;
            }

            return default;
        }
#endif
    }
}