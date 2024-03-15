/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Effects
{
    using Dypsloom.RhythmTimeline.Core;
    using Dypsloom.RhythmTimeline.Core.Input;
    using Dypsloom.RhythmTimeline.Core.Managers;
    using Dypsloom.Shared;
    using UnityEngine;
    using UnityEngine.Events;
    
    public class TrackInputEventReceiver : MonoBehaviour
    {
        [Tooltip("The ID of the track to listen to.")]
        [SerializeField] protected int m_TrackID = -1;
        [Tooltip("Optionally the track object, instead of the track ID.")]
        [SerializeField] protected TrackObject m_TrackObject;
        [Tooltip("An input was pressed on that track.")]
        [SerializeField] protected UnityEvent m_InputPressed;
        [Tooltip("An input was released on that track.")]
        [SerializeField] protected UnityEvent m_InputReleased;

        protected RhythmDirector m_RhythmDirector;
    
        private void Start()
        {
            if (m_RhythmDirector == null) {
                m_RhythmDirector = Toolbox.Get<RhythmDirector>();
            }

            m_RhythmDirector.RhythmProcessor.OnTriggerInputEvent += HandleOnTriggerInputEvent;

            if (m_TrackObject == null) { return; }

            for (int i = 0; i < m_RhythmDirector.TrackObjects.Length; i++) {
                if (m_RhythmDirector.TrackObjects[i] == m_TrackObject) {
                    m_TrackID = i;
                    break;
                }
            }
        }

        private void HandleOnTriggerInputEvent(InputEventData inputEventData)
        {
            if (m_TrackID != -1 && m_TrackID != inputEventData.TrackID) { return; }

            if (inputEventData.Tap) {
                m_InputPressed.Invoke();
            }
            
            if (inputEventData.Release) {
                m_InputReleased.Invoke();
            }
        }
    }
}
