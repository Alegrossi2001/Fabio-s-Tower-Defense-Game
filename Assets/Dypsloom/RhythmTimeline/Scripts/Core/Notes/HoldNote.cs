/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Notes
{
    using Dypsloom.RhythmTimeline.Core.Input;
    using Dypsloom.RhythmTimeline.Core.Playables;
    using UnityEngine;

    /// <summary>
    /// A hold note is a note that must be pressed at a start point and released at an end point.
    /// </summary>
    public class HoldNote : Note
    {
        [Tooltip("The Start note when the input should be pressed.")]
        [SerializeField] protected Transform m_StartNote;
        [Tooltip("The end note when the input should be released.")]
        [SerializeField] protected Transform m_EndNote;
        [Tooltip("The line rendrer used to connect the start and end note.")]
        [SerializeField] protected LineRenderer m_LineRenderer;
        [Tooltip("The color of the line when activated.")]
        [SerializeField] protected Color m_ActiveLineColor = Color.green;
        [Tooltip("The hold not will automatically release when it reaches perfect.")]
        [SerializeField] protected bool m_AutoPerfectRelease;
        [Tooltip("Destroy the not if it was missed, if not the note will still be holdable to get some points at the release.")]
        [SerializeField] protected bool m_RemoveNoteIfMissed = true;

        protected bool m_Holding;
        protected Color m_StartLineColor;

        protected double m_StartHoldTimeOffset;

        /// <summary>
        /// Initialize the note.
        /// </summary>
        /// <param name="rhythmClipData">The rhythm Clip Data.</param>
        public override void Initialize(RhythmClipData rhythmClipData)
        {
            base.Initialize(rhythmClipData);
            m_Holding = false;
            m_LineRenderer.positionCount = 2;
            m_StartLineColor = m_LineRenderer.startColor;
            m_StartHoldTimeOffset = 0;
        }

        /// <summary>
        /// Reset the note when it is returned to the pool.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            m_LineRenderer.startColor = m_StartLineColor;
        }

        /// <summary>
        /// Later update happens after update, the line must be updated after the notes have been moved.
        /// </summary>
        protected virtual void  LateUpdate()
        {
            UpdateLinePositions();
        }

        /// <summary>
        /// The timeline update, updates every frame and in edit mode too.
        /// </summary>
        /// <param name="globalClipStartTime">The offset to the clip start time.</param>
        /// <param name="globalClipEndTime">The offset to the clip stop time</param>
        public override void TimelineUpdate(double globalClipStartTime, double globalClipEndTime)
        {
            base.TimelineUpdate(globalClipStartTime, globalClipEndTime);
            UpdateLinePositions();
        }

        /// <summary>
        /// Update the positions of the line renderer.
        /// </summary>
        private void UpdateLinePositions()
        {
            m_LineRenderer.SetPosition(0, m_StartNote.transform.localPosition);
            m_LineRenderer.SetPosition(1, m_EndNote.transform.localPosition);
        }

        /// <summary>
        /// The note needs to be activated as it is within range of being triggered.
        /// This usually happens when the clip starts.
        /// </summary>
        protected override void ActivateNote()
        {
            base.ActivateNote();
            m_LineRenderer.startColor = m_ActiveLineColor;
        }
    
        /// <summary>
        /// The note was deactivated.
        /// </summary>
        protected override void DeactivateNote()
        {
            base.DeactivateNote();

            if(Application.isPlaying == false){return;}
		
            if (m_IsTriggered == false) {
                gameObject.SetActive(false);
                InvokeNoteTriggerEventMiss();
            }
        }

        /// <summary>
        /// Trigger an input on the note. Detect both tap and release inputs.
        /// </summary>
        /// <param name="inputEventData">The input event data.</param>
        public override void OnTriggerInput(InputEventData inputEventData)
        {
            if (inputEventData.Tap) {
                m_Holding = true;
            
                m_StartNote.position = m_RhythmClipData.TrackObject.EndPoint.position;
            
                var perfectTime = m_RhythmClipData.RhythmDirector.HalfCrochet;
                var timeDifference = TimeFromActivate - perfectTime;

                m_StartHoldTimeOffset = timeDifference;
            }

            if (m_Holding && inputEventData.Release) {
            
                gameObject.SetActive(false);
                m_IsTriggered = true;
            
                var perfectTime = m_RhythmClipData.RhythmDirector.HalfCrochet;
                var timeDifference = TimeFromDeactivate + perfectTime;

                var averageTotalTimeDifference = (m_StartHoldTimeOffset + timeDifference)/2f;
                var timeDifferencePercentage =  Mathf.Abs((float)(100f*averageTotalTimeDifference)) / perfectTime;
                
                InvokeNoteTriggerEvent(inputEventData, timeDifference, (float) timeDifferencePercentage);
                RhythmClipData.TrackObject.RemoveActiveNote(this);
            }
        
        }
    
        /// <summary>
        /// Hybrid update works both in play and edit mode.
        /// </summary>
        /// <param name="timeFromStart">The offset before the start.</param>
        /// <param name="timeFromEnd">The offset before the end.</param>
        protected override void HybridUpdate(double timeFromStart, double timeFromEnd)
        {
            if(Application.isPlaying && (m_ActiveState == ActiveState.PostActive || m_ActiveState == ActiveState.Disabled)){return;}

            var deltaTStart = (float)(timeFromStart - m_RhythmClipData.RhythmDirector.HalfCrochet);
            var deltaTEnd =  (float)(timeFromEnd + m_RhythmClipData.RhythmDirector.HalfCrochet);

            if (m_Holding == false) {
                m_StartNote.position = GetNotePosition(deltaTStart);

                if (Application.isPlaying) {
                    if (m_RemoveNoteIfMissed && timeFromStart > m_RhythmClipData.RhythmDirector.Crochet) {
                        //Force a miss.
                        DeactivateNote();
                        gameObject.SetActive(false);
                    }
                }
            }

            if (m_AutoPerfectRelease && Application.isPlaying && m_Holding && deltaTEnd > 0) {
                //Trigger a release input within code.
                OnTriggerInput(new InputEventData(RhythmClipData.TrackID,1));
                DeactivateNote();
            }

            m_EndNote.position = GetNotePosition(deltaTEnd);
        }
    
        /// <summary>
        /// Get the position of the Note for the delta time.
        /// </summary>
        /// <param name="deltaT">The delta time.</param>
        /// <returns>The position of the note.</returns>
        protected Vector3 GetNotePosition(float deltaT)
        {
            var direction = RhythmClipData.TrackObject.GetNoteDirection(deltaT);
            var distance = deltaT * m_RhythmClipData.RhythmDirector.NoteSpeed;
            var targetPosition = m_RhythmClipData.TrackObject.EndPoint.position;
        
            return targetPosition + (direction * distance);
        }
    }
}