/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Notes
{
	using Dypsloom.RhythmTimeline.Core.Input;
	using Dypsloom.RhythmTimeline.Core.Playables;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// The note trigger event data is used to hold information about the trigger event.
	/// </summary>
	public class NoteTriggerEventData
	{
		public Note Note;
		public InputEventData InputEventData;

		public bool Miss;
	
		public double TriggerDspTime;
		public double DspTimeDifference;
		public float DspTimeDifferencePercentage;

		public void SetTriggerData(InputEventData eventData, double dspTimeDiff, float dspTimeDiffPerc)
		{
			InputEventData = eventData;
			TriggerDspTime = DspTime.AdaptiveTime;
			DspTimeDifference = dspTimeDiff;
			DspTimeDifferencePercentage = dspTimeDiffPerc;
			Miss = false;
		}

		public void SetMiss()
		{
			InputEventData = null;
			TriggerDspTime = DspTime.AdaptiveTime;
			DspTimeDifference = 0;
			DspTimeDifferencePercentage = 100;
			Miss = true;
		}
	}

	/// <summary>
	/// The states for the note.
	/// </summary>
	public enum ActiveState
	{
		Disabled,	// When the Note has not been Initialized yet
		PreActive,	// Between the note being intialized and the active state
		Active,		// While the note is active
		PostActive	// While the note has been deactivated but not reinitialized.
	}

	/// <summary>
	/// The base class for the note component.
	/// </summary>
	public abstract class Note : MonoBehaviour
	{
		public event Action<NoteTriggerEventData> OnNoteTriggerEvent;
		public event Action<Note> OnInitialize;
		public event Action<Note> OnReset;
		public event Action<Note> OnActivate;
		public event Action<Note> OnDeactivate;

		[Tooltip("If set to false the Note will be updated using the rhythm director start DSP time as single truth, instead of the timeline current time.")]
		[SerializeField] protected bool m_UpdateWithTimeline = true;
		[Tooltip("If set to false the Note will activate with time which will give more precision.")]
		[SerializeField] protected bool m_ActivateWithClip = false;
		[Tooltip("Orient the rotation of the Note to match the track rotation?")]
		[SerializeField] protected bool m_OrientToTrack = true;
		[Tooltip("The game object to set active while the clip is running.")]
		[SerializeField] protected GameObject m_SetActiveWhileClipActive;
	
		protected RhythmClipData m_RhythmClipData;

		protected ActiveState m_ActiveState;
		protected bool m_Deactivated;
		protected bool m_IsTriggered;
		protected double m_ActualInitializeTime;
		protected double m_ActualActivateTime;

		protected NoteTriggerEventData m_NoteTriggerEventData;
	
		public ActiveState ActiveState => m_ActiveState;
		public bool IsTriggered => m_IsTriggered;
		public double TrueInitializeTime => m_RhythmClipData.ClipStart - RhythmClipData.RhythmDirector.SpawnTimeRange.x;
		public double TrueActivateTime => m_RhythmClipData.ClipStart;

		public double ActualInitializeTime => m_ActualInitializeTime;
		public double ActualActivateTime => m_ActualActivateTime;
		public double TimeFromActivate => CurrentTime - m_RhythmClipData.ClipStart;
		public double TimeFromDeactivate => CurrentTime - m_RhythmClipData.ClipEnd;

		public RhythmClipData RhythmClipData => m_RhythmClipData;

		public double CurrentTime => 
			m_UpdateWithTimeline || Application.isPlaying == false
				? m_RhythmClipData.RhythmDirector.PlayableDirector.time
				: DspTime.AdaptiveTime - m_RhythmClipData.RhythmDirector.DspSongStartTime;

		/// <summary>
		/// Create a cache the event data on awake.
		/// </summary>
		protected virtual void Awake()
		{
			m_NoteTriggerEventData = new NoteTriggerEventData();
			m_NoteTriggerEventData.Note = this;
		}

		protected virtual void Start()
		{ }

		/// <summary>
		/// Initialize the note using the rhythm clip data.
		/// </summary>
		/// <param name="rhythmClipData">The rhythm clip data.</param>
		public virtual void Initialize(RhythmClipData rhythmClipData)
		{
			if (rhythmClipData.TrackObject == null) {
				Debug.LogWarning("The Track Object cannot be null",gameObject);
				return;
			}
			
			m_RhythmClipData = rhythmClipData;

			if (m_OrientToTrack) {
				transform.rotation = rhythmClipData.TrackObject.StartPoint.rotation;
			} else {
				transform.right = Vector3.left;
			}

			m_IsTriggered = false;
			m_ActiveState = ActiveState.PreActive;

			m_ActualInitializeTime = CurrentTime;
			m_ActualActivateTime =  -1;
			
			InvokeOnInitialize();
		}

		/// <summary>
		/// Invoke the On Initialize event.
		/// </summary>
		protected virtual void InvokeOnInitialize()
		{
			OnInitialize?.Invoke(this);
		}

		/// <summary>
		/// Reset when the note is returned to the pool.
		/// </summary>
		public virtual void Reset()
		{
			m_ActiveState = ActiveState.Disabled;
			InvokeOnReset();
		}

		/// <summary>
		/// Invoked the on reset event.
		/// </summary>
		protected virtual void InvokeOnReset()
		{
			OnReset?.Invoke(this);
		}

		/// <summary>
		/// The clip started.
		/// </summary>
		public virtual void OnClipStart()
		{
			if (m_ActivateWithClip) {
				ActivateNote();
			}
		}

		/// <summary>
		/// The clip stopped.
		/// </summary>
		public virtual void OnClipStop()
		{
			if (m_ActivateWithClip) {
				DeactivateNote();
			}
		}

		/// <summary>
		/// The note needs to be activated as it is within range of being triggered.
		/// This usually happens when the clip starts.
		/// </summary>
		protected virtual void ActivateNote()
		{
			m_ActiveState = ActiveState.Active;
			RhythmClipData.TrackObject.SetActiveNote(this);
			if (m_SetActiveWhileClipActive != null) {
				m_SetActiveWhileClipActive.SetActive(true);
			}
			m_ActualActivateTime = CurrentTime;
			InvokeOnActivate();
		}
		
		/// <summary>
		/// Invoke the on activate event.
		/// </summary>
		protected virtual void InvokeOnActivate()
		{
			OnActivate?.Invoke(this);
		}

		/// <summary>
		/// The note was deactivated.
		/// </summary>
		protected virtual void DeactivateNote()
		{
			m_ActiveState = ActiveState.PostActive;
			RhythmClipData.TrackObject.RemoveActiveNote(this);
			if (m_SetActiveWhileClipActive != null) {
				m_SetActiveWhileClipActive.SetActive(false);
			}

			InvokeOnDeactivate();
		}
		
		/// <summary>
		/// Invoke the on deactivate event.
		/// </summary>
		protected virtual void InvokeOnDeactivate()
		{
			OnDeactivate?.Invoke(this);
		}

		/// <summary>
		/// Trigger an input on the note.
		/// </summary>
		/// <param name="inputEventData">The input event data.</param>
		public abstract void OnTriggerInput(InputEventData inputEventData);

		/// <summary>
		/// Invoke note trigger event.
		/// </summary>
		protected virtual void InvokeNoteTriggerEvent()
		{
			OnNoteTriggerEvent?.Invoke(m_NoteTriggerEventData);
		}
	
		/// <summary>
		/// Invoke the note trigger missed event.
		/// </summary>
		protected virtual void InvokeNoteTriggerEventMiss()
		{
			m_NoteTriggerEventData.SetMiss();
			InvokeNoteTriggerEvent();
		}
	
		/// <summary>
		/// Trigger a note trigger event with the offset.
		/// </summary>
		/// <param name="eventData">The input event data.</param>
		/// <param name="dspTimeDiff">The offset from a perfect hit.</param>
		/// <param name="dspTimeDiffPerc">The offset from a perfect hit in percentage.</param>
		protected virtual void InvokeNoteTriggerEvent(InputEventData eventData, double dspTimeDiff, float dspTimeDiffPerc)
		{
			m_NoteTriggerEventData.SetTriggerData(eventData,dspTimeDiff,dspTimeDiffPerc);
			InvokeNoteTriggerEvent();
		}

		/// <summary>
		/// The timeline update, updates every frame and in edit mode too.
		/// </summary>
		/// <param name="globalClipStartTime">The offset to the clip start time.</param>
		/// <param name="globalClipEndTime">The offset to the clip stop time</param>
		public virtual void TimelineUpdate(double globalClipStartTime, double globalClipEndTime)
		{
			if(m_UpdateWithTimeline == false && Application.isPlaying == false){return;}

			if (m_ActivateWithClip == false) {
				if (m_ActiveState == ActiveState.Active && globalClipEndTime >= 0) {
					DeactivateNote();
				} else if(m_ActiveState == ActiveState.PreActive && globalClipStartTime >= 0) {
					ActivateNote();
				}
			}
		
			HybridUpdate( globalClipStartTime, globalClipEndTime );
		}
	
		/// <summary>
		/// Default update can be used instead of timeline update to sync with DSP adaptive time.
		/// </summary>
		protected virtual void Update()
		{
			if(m_UpdateWithTimeline){return;}
		
			//Debug.Log("time line update dsp"+m_DspGlobalStartTime +" end "+m_DspGlobalEndTime);
			if (m_ActivateWithClip == false) {
				if (m_ActiveState == ActiveState.Active && TimeFromDeactivate >= 0) {
					DeactivateNote();
				} else if(m_ActiveState == ActiveState.PreActive && TimeFromActivate >= 0) {
					ActivateNote();
				}
			}
		
			HybridUpdate(TimeFromActivate, TimeFromDeactivate);
		}

		/// <summary>
		/// Hybrid update works both in play and edit mode.
		/// </summary>
		/// <param name="timeFromStart">The offset before the start.</param>
		/// <param name="timeFromEnd">The offset before the end.</param>
		protected abstract void HybridUpdate(double timeFromStart, double timeFromEnd);

	}
}