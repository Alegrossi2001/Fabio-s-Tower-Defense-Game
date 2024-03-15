/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Playables
{
    using UnityEngine;
    using UnityEngine.Playables;
    using System;
    using Dypsloom.RhythmTimeline.Core.Notes;

    [Serializable]
    public class RhythmBehaviour : PlayableBehaviour
    {
        [Tooltip("The note definition.")]
        [SerializeField] protected NoteDefinition m_NoteDefinition;

        public NoteDefinition NoteDefinition => m_NoteDefinition;
        public RhythmClip RhythmClip { get; set; }
        public RhythmClipData RhythmClipData { get => RhythmClip.RhythmClipData; }

        protected bool m_IsNoteSpawned;
        protected Note m_Note;

        protected bool m_MissingDefinition;
    
        public override void OnPlayableCreate (Playable playable)
        { }

        public void SetNoteDefinition(NoteDefinition noteDefinition)
        {
            m_NoteDefinition = noteDefinition;
        }

        //Takes care of instantiating the GameObject
        public override void OnGraphStart(Playable playable)
        {
            m_MissingDefinition = m_NoteDefinition == null || m_NoteDefinition.NotePrefab == null || m_NoteDefinition.NotePrefab.GetComponent<Note>() == null;
            if (m_MissingDefinition) {
                Debug.LogWarning($"The Rhythm Object Definition {m_NoteDefinition} for this clip is missing, or its prefab is missing or the prefab does note contain a Note component.");
            }
        }

        protected virtual void SpawnNote()
        {
            //create the associated prefab
            if (m_MissingDefinition) { return; }
            if (m_IsNoteSpawned) { return; }

            m_Note = RhythmClipData.RhythmDirector.RhythmProcessor.CreateNewNote(m_NoteDefinition, RhythmClip);
            m_IsNoteSpawned = true;
        }
    
        protected virtual void RemoveNote()
        {
            if (m_IsNoteSpawned == false) { return; }
        
            RhythmClipData.RhythmDirector.RhythmProcessor.DestroyNote(m_Note);
            m_IsNoteSpawned = false;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if(m_IsNoteSpawned == false){ return; }
        
            m_Note.OnClipStart();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!Application.isPlaying) {
                return;
            }
 
            var duration = playable.GetDuration();
            var time = playable.GetTime();
            var count = time + info.deltaTime;
 
            if ((info.effectivePlayState == PlayState.Paused && count > duration) || Mathf.Approximately((float)time, (float)duration)) {
                // Execute your finishing logic here:
                m_Note.OnClipStop();
            }
        }

        public void MixerProcessFrame(Playable thisPlayable, FrameData info, object playerData, double timelineCurrentTime)
        {
            if(m_MissingDefinition){ return; }
            /* Calculate the clip time starting from the actual Timeline time
            the only reason why we need this is because we need it to be able to be negative or past the clip's duration,
            so we can handle bullets also after the clip ends
            thisPlayable.GetTime() only gives time constrained to the clip duration */
#if UNITY_EDITOR
            // Update the BPM in case it was changed in the inspector.
            RhythmClipData.RhythmDirector.RefreshBpm();
#endif
        
            var globalClipStartTime = timelineCurrentTime - RhythmClipData.ClipStart;
            var globalClipEndTime = timelineCurrentTime - RhythmClipData.ClipEnd;

            var timeRange = RhythmClipData.RhythmDirector.SpawnTimeRange;
            //Debug.Log($"{m_GlobalClipTime} -> [{-timeRange.x} ; {timeRange.y}]");
            if (!(globalClipStartTime >= -timeRange.x) || !(globalClipEndTime < timeRange.y)) {
                //hide note.
                if (m_IsNoteSpawned) { RemoveNote(); }

                return;
            }

            //show note.
            if (m_IsNoteSpawned == false) { SpawnNote(); }
        
            m_Note.TimelineUpdate(globalClipStartTime, globalClipEndTime);
        }

        public override void OnGraphStop(Playable playable)
        { }

        //Takes care of destroying the GameObject, if it still exists
        public override void OnPlayableDestroy(Playable playable)
        {
            //Debug.Log("Playable Destroy");
            RemoveNote();
        }
    }
}
