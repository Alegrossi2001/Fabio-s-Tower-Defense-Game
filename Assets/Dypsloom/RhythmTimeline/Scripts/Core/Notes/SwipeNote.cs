/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Notes
{
    using UnityEngine;
    using Dypsloom.RhythmTimeline.Core.Input;
    using UnityEngine.Serialization;

    /// <summary>
    /// A swipe Note is similar to a tap note except its input must have a direction.
    /// </summary>
    public class SwipeNote : TapNote
    {
        [FormerlySerializedAs("m_SwapDirection")]
        [Tooltip("The Swipe direction in 2D.")]
        [SerializeField] protected Vector2 m_SwipeDirection;
        [Tooltip("The Angle of tolerance when swiping in a direction.")]
        [SerializeField] protected float m_AngleTolerance = 30;
        [Tooltip("Lowest score possible when the swipe was in the wrong direction? or ignore wrong direction swipes?.")]
        [SerializeField] protected bool m_FailOnWrongDirectionSwipe = false;
    
        /// <summary>
        /// Trigger an input on the note. Detect swipes.
        /// </summary>
        /// <param name="inputEventData">The input event data.</param>
        public override void OnTriggerInput(InputEventData inputEventData)
        {
            if (!inputEventData.Swipe) { return; }

            var swipeAngleOffset = Vector2.Angle(m_SwipeDirection, inputEventData.Direction);

            if (swipeAngleOffset > m_AngleTolerance) {
                
                if(m_FailOnWrongDirectionSwipe == false){return;}
                
                //Fail input should be a bad.
                
                gameObject.SetActive(false);
                m_IsTriggered = true;
                
                InvokeNoteTriggerEvent(inputEventData, m_RhythmClipData.RealDuration, 100);
                RhythmClipData.TrackObject.RemoveActiveNote(this);
                return;
            }
        
            gameObject.SetActive(false);
            m_IsTriggered = true;
            
            var perfectTime = m_RhythmClipData.RealDuration / 2f;
            var timeDifference = TimeFromActivate - perfectTime;
            var timeDifferencePercentage =  Mathf.Abs((float)(100f*timeDifference)) / perfectTime;
		
            InvokeNoteTriggerEvent(inputEventData, timeDifference, (float) timeDifferencePercentage);
            RhythmClipData.TrackObject.RemoveActiveNote(this);
        }
    }
}