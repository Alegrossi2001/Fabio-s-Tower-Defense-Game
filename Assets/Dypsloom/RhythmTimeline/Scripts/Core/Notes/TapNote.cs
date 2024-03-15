/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Notes
{
    using UnityEngine;
    using Dypsloom.RhythmTimeline.Core.Input;
    using Dypsloom.RhythmTimeline.Core.Playables;

    /// <summary>
    /// The Tap Note detects a single press input.
    /// </summary>
    public class TapNote : Note
    {
        /// <summary>
        /// The note is initialized when it is added to the top of a track.
        /// </summary>
        /// <param name="rhythmClipData">The rhythm clip data.</param>
        public override void Initialize(RhythmClipData rhythmClipData)
        {
            base.Initialize(rhythmClipData);
        }
        
        /// <summary>
        /// Reset when the note is returned to the pool.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
        }
        
        /// <summary>
        /// The note needs to be activated as it is within range of being triggered.
        /// This usually happens when the clip starts.
        /// </summary>
        protected override void ActivateNote()
        {
            base.ActivateNote();
        }

        /// <summary>
        /// The note needs to be deactivated when it is out of range from being triggered.
        /// This usually happens when the clip ends.
        /// </summary>
        protected override void DeactivateNote()
        {
            base.DeactivateNote();

            //Only send the trigger miss event during play mode.
            if(Application.isPlaying == false){return;}
            
            if (m_IsTriggered == false) {
                InvokeNoteTriggerEventMiss();
            }
        }
    
        /// <summary>
        /// An input was triggered on this note.
        /// The input event data has the information about what type of input was triggered.
        /// </summary>
        /// <param name="inputEventData">The input event data.</param>
        public override void OnTriggerInput(InputEventData inputEventData)
        {
            //Since this is a tap note, only deal with tap inputs.
            if (!inputEventData.Tap) { return; }

            //The gameobject can be set to active false. It is returned to the pool automatically when reset.
            gameObject.SetActive(false);
            m_IsTriggered = true;

            //You may compute the perfect time anyway you want.
            //In this case the perfect time is half of the clip.
            var perfectTime = m_RhythmClipData.RealDuration / 2f;
            var timeDifference = TimeFromActivate - perfectTime;
            var timeDifferencePercentage =  Mathf.Abs((float)(100f*timeDifference)) / perfectTime;
            
            //Send a trigger event such that the score system can listen to it.
            InvokeNoteTriggerEvent(inputEventData, timeDifference, (float) timeDifferencePercentage);
            RhythmClipData.TrackObject.RemoveActiveNote(this);
        }
    
        /// <summary>
        /// Hybrid Update is updated both in play mode, by update or timeline, and edit mode by the timeline. 
        /// </summary>
        /// <param name="timeFromStart">The time from reaching the start of the clip.</param>
        /// <param name="timeFromEnd">The time from reaching the end of the clip.</param>
        protected override void HybridUpdate(double timeFromStart, double timeFromEnd)
        {
            //Compute the perfect timing.
            var perfectTime = m_RhythmClipData.RealDuration / 2f;
            var deltaT = (float)(timeFromStart - perfectTime);

            //Compute the position of the note using the delta T from the perfect timing.
            //Here we use the direction of the track given at delta T.
            //You can easily curve all your notes to any trajectory, not just straight lines, by customizing the TrackObjects.
            //Here the target position is found using the track object end position.
            var direction = RhythmClipData.TrackObject.GetNoteDirection(deltaT);
            var distance = deltaT * m_RhythmClipData.RhythmDirector.NoteSpeed;
            var targetPosition = m_RhythmClipData.TrackObject.EndPoint.position;
        
            //Using those parameters we can easily compute the new position of the note at any time.
            var newPosition = targetPosition + (direction * distance);
            transform.position = newPosition;
        }
    }
}