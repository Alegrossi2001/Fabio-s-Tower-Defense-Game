namespace Dypsloom.RhythmTimeline.Effects
{
    using Dypsloom.RhythmTimeline.Core;
    using Dypsloom.RhythmTimeline.Core.Input;
    using Dypsloom.RhythmTimeline.Core.Managers;
    using Dypsloom.RhythmTimeline.Core.Notes;
    using Dypsloom.Shared;
    using UnityEngine;
    using UnityEngine.Events;

    public class TrackNoteEventReceiver : MonoBehaviour
    {
        [Tooltip("The ID of the track to listen to.")]
        [SerializeField] protected int m_TrackID = -1;
        [Tooltip("Optionally the track object instead of the track ID.")]
        [SerializeField] protected TrackObject m_TrackObject;
        [Tooltip("Event when an note is activated on that track.")]
        [SerializeField] protected UnityEvent m_OnNoteActivate;
        [Tooltip("Event when a note is deactivated on that track.")]
        [SerializeField] protected UnityEvent m_OnNoteDeactivate;
        [Tooltip("Event when the note is triggered.")]
        [SerializeField] protected UnityEvent m_OnNoteTriggered;
        [Tooltip("Event when the note is missed.")]
        [SerializeField] protected UnityEvent m_OnNoteTriggeredMiss;

        protected RhythmDirector m_RhythmDirector;

        private void Start()
        {
            if (m_RhythmDirector == null) {
                m_RhythmDirector = Toolbox.Get<RhythmDirector>();
            }

            m_RhythmDirector.RhythmProcessor.OnNoteActivateEvent += HandleOnNoteActivateEvent;
            m_RhythmDirector.RhythmProcessor.OnNoteDeactivateEvent += HandleOnNoteDeactivateEvent;
            m_RhythmDirector.RhythmProcessor.OnNoteTriggerEvent += HandleOnNoteTriggeredEvent;

            if (m_TrackObject == null) { return; }

            for (int i = 0; i < m_RhythmDirector.TrackObjects.Length; i++) {
                if (m_RhythmDirector.TrackObjects[i] == m_TrackObject) {
                    m_TrackID = i;
                    break;
                }
            }
        }

        private void HandleOnNoteActivateEvent(Note note)
        {
            if (m_TrackID != -1 && m_TrackID != note.RhythmClipData.TrackID) { return; }

            
            m_OnNoteActivate.Invoke();
        }
        
        private void HandleOnNoteDeactivateEvent(Note note)
        {
            if (m_TrackID != -1 && m_TrackID != note.RhythmClipData.TrackID) { return; }
            
            m_OnNoteDeactivate.Invoke();
        }
        
        private void HandleOnNoteTriggeredEvent(NoteTriggerEventData noteTriggerEventData)
        {
            if (m_TrackID != -1 && m_TrackID != noteTriggerEventData.Note.RhythmClipData.TrackID) { return; }

            if (noteTriggerEventData.Miss) {
                m_OnNoteTriggeredMiss.Invoke();
            } else {
                m_OnNoteTriggered.Invoke();
            }
        }
    }
}