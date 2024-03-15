/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

using Dypsloom.RhythmTimeline.Core.Notes;

namespace Dypsloom.RhythmTimeline.Core.Playables
{
    using Dypsloom.RhythmTimeline.Core.Managers;
    using System.ComponentModel;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    [TrackColor(0.7846687f, 0.4103774f, 1f)]
    [TrackClipType(typeof(RhythmClip))]
    [DisplayName("Rhythm/Rhythm Track")]
    public class RhythmTrack : TrackAsset
    {
        [Tooltip("The Rhythm Track ID.")]
        [SerializeField] protected int m_ID;

        public int ID => m_ID;

        protected RhythmDirector m_RhythmDirector;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            //Debug.Log("Create Track Mixer");
            var playable = ScriptPlayable<RhythmMixerBehaviour>.Create(graph, inputCount);

            //store the lane starting position in the clips to allow correct calculation of the paths
            //store start times to allow bullet/ship calculations past the clip's length
            if (m_RhythmDirector == null) {

                m_RhythmDirector = go.GetComponent<RhythmDirector>();

                if (m_RhythmDirector == null) {
                    Debug.LogError("The Rhythm Director is missing from the Rhythm Track Binding.");
                    return playable;
                }
            }
        
            //Set the BPM
            m_RhythmDirector.RefreshBpm();

            foreach (var clip in m_Clips)
            {
                var rhythmClip = clip.asset as RhythmClip;
                if (m_RhythmDirector.TrackObjects == null || m_RhythmDirector.TrackObjects.Length == 0 || m_ID >= m_RhythmDirector.TrackObjects.Length) {
                    Debug.LogError("The Rhythm Director is Missing a Track Data for index: "+m_ID);
                    continue;
                }
            
                var clipData = new RhythmClipData(rhythmClip,
                    m_RhythmDirector,
                    m_ID,
                    clip.start,
                    clip.duration);

                rhythmClip.RhythmClipData = clipData;

                SetClipDuration(rhythmClip, clip);
            }

            return playable;
        }

        protected virtual void SetClipDuration(RhythmClip rhythmClip, TimelineClip clip)
        {
            if(rhythmClip?.RhythmPlayableBehaviour?.NoteDefinition == null){ return; }

            rhythmClip.RhythmPlayableBehaviour.NoteDefinition.SetClipDuration(rhythmClip, clip);
        }
    }
}