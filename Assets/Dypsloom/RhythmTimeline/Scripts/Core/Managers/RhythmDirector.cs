/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Managers
{
    using Dypsloom.Shared;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Playables;
    using UnityEngine.Serialization;
    using UnityEngine.Timeline;

    /// <summary>
    /// The Rhythm Director is a component which controls the Playable Director
    /// binding the Rhythm Tracks to Track Objects and making sure the
    /// Rhythm Timeline Asset is playable in play mode and can be previewed in Edit mode.
    /// </summary>
    public class RhythmDirector : MonoBehaviour
    {
        public event Action OnSongPlay;
        public event Action OnSongEnd;
        
        [Header("Version 1.3.0")]
        [Space]
        
        [Tooltip("The Rhythm Processor.")]
        [SerializeField] protected RhythmProcessor m_RhythmProcessor;
        [Tooltip("The Playable Director.")]
        [SerializeField] protected PlayableDirector m_PlayableDirector;

        [Tooltip("The x means the time the note should be spawned before it reaches the target. " +
                 "The y means the time the note should disappear after reaching target")]
        [SerializeField] protected Vector2 m_SpawnTimeRange = new Vector2(9,3);
        [Tooltip("The bpm of the song (this is usually assigned by the Song Timeline).")]
        [SerializeField] protected float m_Bpm = 120;
        [Tooltip("The Note Speed.")]
        [SerializeField] protected float m_NoteSpeed = 2;
        [Tooltip("Should the note speed scale with the BPM?")]
        [SerializeField] protected bool m_ScaleNoteSpeedToBpm;
        [Tooltip("The latency to add to the audio tracks of the song timeline.")]
        [SerializeField] protected float m_LatencyCompensation;
        [Tooltip("Play the song on awake.")]
        [SerializeField] protected bool m_PlayOnStart = false;

        [Header("TimeLine Bindings")]
        [Tooltip("The Audio Sources to bind to the Audio Tracks.")]
        [SerializeField] protected AudioSource[] m_AudioSources;
        [Tooltip("The Track Objects to bind to the Rhythm Tracks.")]
        [SerializeField] protected TrackObject[] m_TrackObjects;
        
        protected int m_AudioTracksCount = 0;
    
        protected RhythmTimelineAsset m_SongTimelineAsset;
        protected bool m_IsPlaying;
        protected double m_PausedTime;
        protected double m_DspSongStartTime;
        protected float m_RealNoteSpeed;

        public bool IsPlaying => m_IsPlaying;

        public RhythmProcessor RhythmProcessor => m_RhythmProcessor;
        public RhythmTimelineAsset SongTimelineAsset
        {
            get
            {
                if (Application.isPlaying == false) {
                    return m_PlayableDirector.playableAsset as RhythmTimelineAsset;
                }
                
                return m_SongTimelineAsset != null
                    ? m_SongTimelineAsset
                    : m_PlayableDirector.playableAsset as RhythmTimelineAsset;
            }
        }
        public PlayableDirector PlayableDirector => m_PlayableDirector;
        public float NoteSpeed => m_RealNoteSpeed;
        public virtual double AudioDelay => m_LatencyCompensation;
        public float Bpm => m_Bpm;
        public float Crochet => 60f / m_Bpm;
        public float HalfCrochet => 30f / m_Bpm;
        public float QuarterCrochet => 15f / m_Bpm;
        public TrackObject[] TrackObjects => m_TrackObjects;
        public Vector2 SpawnTimeRange => m_SpawnTimeRange;
        public double DspSongStartTime => m_DspSongStartTime;

        public void RefreshBpm()
        {
            SetBpm(SongTimelineAsset.Bpm);
        }
        
        public void SetBpm(float newBpm)
        {
            m_Bpm = newBpm;
            if (m_ScaleNoteSpeedToBpm) {
                m_RealNoteSpeed =m_NoteSpeed * m_Bpm;
            } else {
                m_RealNoteSpeed = m_NoteSpeed; 
            }
        }
    
        private void Awake()
        {
            Toolbox.Set(this);
            m_PlayableDirector.timeUpdateMode = DirectorUpdateMode.DSPClock;
            m_PlayableDirector.stopped += HandleSongEnded;
        }

        private void Start()
        {
            if (m_PlayOnStart) {
                PlaySong(m_PlayableDirector.playableAsset as RhythmTimelineAsset);
            }
        }

        public void PlaySong(RhythmTimelineAsset songTimeLine)
        {
            m_SongTimelineAsset = songTimeLine;
            m_PlayableDirector.playableAsset = m_SongTimelineAsset;

            RefreshBpm();

            SetupTrackBindings();

            // rebuild for runtime playing
            m_PlayableDirector.RebuildGraph();
            m_PlayableDirector.time = 0.0;
            m_PlayableDirector.Play();
            m_DspSongStartTime = DspTime.AdaptiveTime;
            m_IsPlaying = true;
            
            OnSongPlay?.Invoke();
        }

        protected virtual void SetupTrackBindings()
        {
            var outputTracks = m_SongTimelineAsset.GetOutputTracks();
            foreach (var track in outputTracks) {
            
                if (track  is AudioTrack audioTrack) { SetUpAudioTrackBinding(audioTrack); }
            
            }
        }

        protected void SetUpAudioTrackBinding(AudioTrack audioTrack)
        {
            //Delay the audio tracks to offset the visuals from the audio slightly to compensate latency.
            var audioClips = audioTrack.GetClips();
            foreach (var audioClip in audioClips) {
                audioClip.start += AudioDelay;
            }
            
            m_PlayableDirector.SetGenericBinding(audioTrack,m_AudioSources[m_AudioTracksCount]);
            m_AudioTracksCount++;
        }

        protected void HandleSongEnded(PlayableDirector playableDirector)
        {
            EndSong();
        }

        public void EndSong()
        {
            if (m_IsPlaying == false) { return; }
        
            m_PlayableDirector.Stop();
            
            ClearActiveNotes();

            m_AudioTracksCount = 0;
        
            //Put back the audio track to it's original place
            var outputTracks = m_SongTimelineAsset.GetOutputTracks();
            foreach (var track in outputTracks) {
                if (track.GetType() == typeof(AudioTrack)) {
                    var audioClips = track.GetClips();
                    foreach (var audioClip in audioClips) {
                        audioClip.start -= AudioDelay;
                    }
                }
            }

            m_PlayableDirector.RebuildGraph();
            m_IsPlaying = false;

            OnSongEnd?.Invoke();
        }

        public virtual void ClearActiveNotes()
        {
            if(m_TrackObjects == null){ return; }
            
            // Clear the active notes on the track objects.
            for (int i = 0; i < m_TrackObjects.Length; i++) { m_TrackObjects[i].ClearNotes(); }
        }

        public void Pause()
        {
            if (m_PlayableDirector == null) {
                return;
            }
            
            //Do not use m_PlayableDirector.Pause() because it stops the playable clips.
            m_PausedTime = m_PlayableDirector.time;
            var count = m_PlayableDirector.playableGraph.GetRootPlayableCount();
            for (int i = 0; i < count; i++) {
                m_PlayableDirector.playableGraph.GetRootPlayable(i).SetSpeed(0);
            }
        }
    
        public void UnPause()
        {
            if (m_PlayableDirector == null) {
                return;
            }

            //Do not use m_PlayableDirector.Play() because pause is not used.
            m_PlayableDirector.time = m_PausedTime;
            var count = m_PlayableDirector.playableGraph.GetRootPlayableCount();
            for (int i = 0; i < count; i++) {
                m_PlayableDirector.playableGraph.GetRootPlayable(i).SetSpeed(1);
            }
        }
    }
}
