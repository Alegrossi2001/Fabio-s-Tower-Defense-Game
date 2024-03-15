/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Notes
{
    using Dypsloom.RhythmTimeline.Core.Input;
    using Dypsloom.RhythmTimeline.Core.Playables;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// The Counter Note is pressed multiple times as fast as possible during a limited time.
    /// </summary>
    public class CounterNote : Note
    {
        [FormerlySerializedAs("m_TmpText")]
        [Tooltip("The Counter Text.")]
        [SerializeField] protected TMP_Text m_CounterText;
        
        protected int m_StartCounter;
        protected int m_Counter;  
    
        /// <summary>
        /// Initialize the Note.
        /// </summary>
        /// <param name="rhythmClipData">The rhythm clip data.</param>
        public override void Initialize(RhythmClipData rhythmClipData)
        {
            base.Initialize(rhythmClipData);
            m_StartCounter = m_RhythmClipData.ClipParameters.IntParameter;
            SetCounter(m_StartCounter);
        }
    
        /// <summary>
        /// Set the number of times the note must be tapped
        /// </summary>
        /// <param name="counter">The amount of times to tap.</param>
        protected void SetCounter(int counter)
        {
            m_Counter = counter;
            m_CounterText.text = m_Counter.ToString();
        }

        /// <summary>
        /// The note was deactivated.
        /// </summary>
        protected override void DeactivateNote()
        {
            base.DeactivateNote();

            if(Application.isPlaying == false){return;}
		
            if (m_IsTriggered == false) {
                InvokeNoteTriggerEventMiss();
            }else if (m_Counter > 0) {
                var percentage = 100*(m_Counter / (float)m_StartCounter);
                InvokeNoteTriggerEvent(null, m_RhythmClipData.RhythmDirector.Crochet, percentage);
            }
        }
    
        /// <summary>
        /// Trigger an input on the note. Detect taps.
        /// </summary>
        /// <param name="inputEventData">The input event data.</param>
        public override void OnTriggerInput(InputEventData inputEventData)
        {
            if (!inputEventData.Tap) { return; }
        
            m_IsTriggered = true;
        
            SetCounter(m_Counter - 1);
            if (m_Counter <= 0) {
                gameObject.SetActive(false);
                InvokeNoteTriggerEvent(inputEventData, 0, 0);
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
            var deltaTStart = (float)(timeFromStart - m_RhythmClipData.RhythmDirector.HalfCrochet);
            var deltaTEnd =  (float)(timeFromEnd + m_RhythmClipData.RhythmDirector.HalfCrochet);

            Vector3 newPosition;
            if (timeFromStart < m_RhythmClipData.RhythmDirector.HalfCrochet) {
                //Move
                newPosition = GetNotePosition(deltaTStart);
            }else if (timeFromEnd < -m_RhythmClipData.RhythmDirector.HalfCrochet) {
                //Wait
                newPosition = GetNotePosition(0);
            }else {
                //Move Again
                newPosition = GetNotePosition(deltaTEnd);
            }
        
            transform.position = newPosition;
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