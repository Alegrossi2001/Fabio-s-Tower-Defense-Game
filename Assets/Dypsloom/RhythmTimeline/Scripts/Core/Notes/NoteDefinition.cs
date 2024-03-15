/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Notes
{
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.Timeline;
	using Dypsloom.RhythmTimeline.Core.Playables;

	[System.Serializable]
	[CreateAssetMenu(fileName = "My Note Definition", menuName = "Dypsloom/Rhythm Timeline/Note Definition", order = 1)]
	public class NoteDefinition : ScriptableObject
	{
		public enum ClipDurationType
		{
			Free,
			Crochet,
			HalfCrochet,
			QuarterCrochet,
			ScaledCrochet,
		}
	
		[Tooltip("The Note Prefab.")]
		[SerializeField] protected GameObject m_NotePrefab;
		[Tooltip("The type of clip duration.")]
		[SerializeField] protected ClipDurationType m_ClipDuration;
		[Tooltip("The scale factor used when the Scaled Crochet option is selected.")]
		[SerializeField] protected float m_ScaledCrochetValue = 1.2f;

		public GameObject NotePrefab => m_NotePrefab;
		public ClipDurationType ClipDuration => m_ClipDuration;
	
#if UNITY_EDITOR
		[Tooltip("The settings used to customize the clip in the editor.")]
		[SerializeField] protected RhythmClipEditorSettings m_RhythmClipEditorSettings;
		public RhythmClipEditorSettings RhythmClipEditorSettings => m_RhythmClipEditorSettings;
#endif
	

		public virtual void SetClipDuration(RhythmClip rhythmClip, TimelineClip clip)
		{
			if(m_ClipDuration == ClipDurationType.Free){return;}
		
			if (m_ClipDuration == ClipDurationType.HalfCrochet) {
				clip.duration = rhythmClip.RhythmClipData.RhythmDirector.HalfCrochet;
				return;
			}
		
			if (m_ClipDuration == ClipDurationType.Crochet) {
				clip.duration = rhythmClip.RhythmClipData.RhythmDirector.Crochet;
				return;
			}
		
			if (m_ClipDuration == ClipDurationType.QuarterCrochet) {
				clip.duration = rhythmClip.RhythmClipData.RhythmDirector.QuarterCrochet;
				return;
			}
		
			if (m_ClipDuration == ClipDurationType.ScaledCrochet) {
				clip.duration = rhythmClip.RhythmClipData.RhythmDirector.Crochet * m_ScaledCrochetValue;
				return;
			}
		}
	}
}