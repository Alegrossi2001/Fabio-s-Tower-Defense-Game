/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core
{
    using Dypsloom.RhythmTimeline.Core.Playables;
    using Dypsloom.RhythmTimeline.Scoring;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Timeline;

    [System.Serializable]
    [CreateAssetMenu(fileName = "RhythmTimelineAsset", menuName = "Dypsloom/Rhythm Timeline/Rhythm Timeline Asset", order = 1)]
    public class RhythmTimelineAsset : TimelineAsset
    {
        [Tooltip("Beats per minute.")]
        [SerializeField] protected float m_Bpm = 120;
        [Tooltip("Use this specific Note speed instead of the note speed defined in the rhythm director.")]
        [SerializeField] protected bool m_OverrideNoteSpeed = false;
        [Tooltip("The Note Speed (only if the override note speed is checked).")]
        [SerializeField] protected float m_NoteSpeed = 3;
        [Tooltip("The full name of the song.")]
        [SerializeField] protected string m_FullName;
        [Tooltip("The Author.")]
        [SerializeField] protected string m_Authour;
        [TextArea(3,10)] 
        [Tooltip("The song description.")] 
        [SerializeField] protected string m_Description;
        [Tooltip("The Difficulty of the song.")]
        [SerializeField] protected int m_Dificulty;
        [Tooltip("The main audio clip, used to preview the song.")]
        [SerializeField] protected AudioClip m_AudioClip;
        [Tooltip("The score at the start of the song")]
        [SerializeField] protected float m_StartScore = 0;
        [Tooltip("The maximum score for this song, (Will automatically compute using note count if max score is 0 or lower)")]
        [SerializeField] protected float m_MaxScore = -1;
        [Tooltip("The minimum score for this song")]
        [SerializeField] protected float m_MinScore = 0;
        [Tooltip("If true the score will never be able to go higher than the max score")]
        [SerializeField] protected bool m_PreventMaxScoreOvershoot = false;
        [Tooltip("If true the score will never be able to go higher than the max score")]
        [SerializeField] protected bool m_PreventMinScoreOvershoot = false;
        [Tooltip("The high score.")]
        [SerializeField] protected ScoreData m_HighScore;
        
        public float Bpm => m_Bpm;
        public bool OverrideNoteSpeed => m_OverrideNoteSpeed;
        public float NoteSpeed => m_NoteSpeed;
        public string FullName => m_FullName;
        public string Authour => m_Authour;
        public string Description => m_Description;
        public int Dificulty => m_Dificulty;
        public AudioClip AudioClip => m_AudioClip;
        public ScoreData HighScore => m_HighScore;

        public float Crochet => 60f / m_Bpm;
        public float HalfCrochet => 30f / m_Bpm;
        public float QuarterCrochet => 15f / m_Bpm;
        
        public int RhythmTrackCount => RhythmClips.Count;
        
        public float StartScore => m_StartScore;
        public float MaxScore => m_MaxScore;
        public float MinScore => m_MinScore;
        public bool PreventMaxScoreOvershoot => m_PreventMaxScoreOvershoot;
        public bool PreventMinScoreOvershoot => m_PreventMinScoreOvershoot;

        public void SetHighScore(ScoreData score)
        {
            m_HighScore = score;
        }
    
        public int RhythmClipCount
        {
            get
            {
                var count = 0;
                var trackCount = RhythmClips.Count;
                for (int i = 0; i < trackCount; i++) { count += m_Beats[i].Count; }

                return count;
            }
        }
    
        [NonSerialized] protected IReadOnlyList<IReadOnlyList<TimelineClip>> m_Beats;

        public IReadOnlyList<IReadOnlyList<TimelineClip>> RhythmClips
        {
            get
            {
                if (m_Beats == null) {

                    var newBeatLists = new List<IReadOnlyList<TimelineClip>>();
                
                    var outputTracks = GetOutputTracks();

                    foreach (var track in outputTracks) {
                        if (track is RhythmTrack rhythmBeatTrack) {
                            var beatList = GetClipsInTrack(rhythmBeatTrack);
                            newBeatLists.Add(beatList);
                        }
                    }

                    m_Beats = newBeatLists;
                }

                return m_Beats;
            }
        }

        protected IReadOnlyList<TimelineClip> GetClipsInTrack(RhythmTrack beatTrack)
        {
            var beatList = new List<TimelineClip>();

            var clips = beatTrack.GetClips();
            foreach (var clip in clips) {
                if (clip.asset is RhythmClip == false) { continue; }
                beatList.Add(clip);
            }
        
            beatList.Sort( (x,y) => x.start.CompareTo(y.start));

            return beatList;
        }
    }
}