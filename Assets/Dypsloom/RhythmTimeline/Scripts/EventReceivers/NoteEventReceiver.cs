namespace Dypsloom.RhythmTimeline.Effects
{
    using Dypsloom.RhythmTimeline.Core;
    using Dypsloom.RhythmTimeline.Core.Managers;
    using Dypsloom.RhythmTimeline.Core.Notes;
    using Dypsloom.Shared;
    using UnityEngine;
    using UnityEngine.Events;

    public class NoteEventReceiver : MonoBehaviour
    {
        [Tooltip("The note to listen the events on.")]
        [SerializeField] protected Note m_Note;
        [Tooltip("The event when the note is activated.")]
        [SerializeField] protected UnityEvent m_OnNoteActivate;
        [Tooltip("The event when the nore is deactivated.")]
        [SerializeField] protected UnityEvent m_OnNoteDeactivate;
        [Tooltip("The event when the note is triggered.")]
        [SerializeField] protected UnityEvent m_OnNoteTriggered;
        [Tooltip("The event when the note was missed.")]
        [SerializeField] protected UnityEvent m_OnNoteTriggeredMiss;

        private void Start()
        {
            if (m_Note == null) {
                m_Note = GetComponent<Note>();
            }
            
            if (m_Note == null) { return; }

            m_Note.OnActivate += HandleOnNoteActivateEvent;
            m_Note.OnDeactivate += HandleOnNoteDeactivateEvent;
            m_Note.OnNoteTriggerEvent += HandleOnNoteTriggeredEvent;
        }

        protected virtual void HandleOnNoteActivateEvent(Note note)
        {
            m_OnNoteActivate.Invoke();
        }
        
        protected virtual void HandleOnNoteDeactivateEvent(Note note)
        {
            m_OnNoteDeactivate.Invoke();
        }
        
        protected virtual void HandleOnNoteTriggeredEvent(NoteTriggerEventData noteTriggerEventData)
        {
            if (noteTriggerEventData.Miss) {
                m_OnNoteTriggeredMiss.Invoke();
            } else {
                m_OnNoteTriggered.Invoke();
            }
        }
    }
}