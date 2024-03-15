/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Playables
{
    using Dypsloom.RhythmTimeline.Core.Managers;
    using System;
    using System.ComponentModel;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;
    using Object = UnityEngine.Object;

    [Serializable]
    //[DisplayName("Rhythm/Rhythm Clip")]
    public class RhythmClip : PlayableAsset, ITimelineClipAsset
    {
        [Tooltip("The Rhythm Playable Behaviour.")]
        [SerializeField] protected RhythmBehaviour m_RhythmPlayableBehaviour = new RhythmBehaviour();
        [Tooltip("The Rhythm Clip parameters.")]
        [SerializeField] protected RhythmClipParameters m_ClipParameters;
        [Tooltip("The rhythm clip data.")]
        [SerializeField] [HideInInspector] protected RhythmClipData m_RhythmClipData;

        public RhythmClipData RhythmClipData { get => m_RhythmClipData; set => m_RhythmClipData = value; }
        public RhythmBehaviour RhythmPlayableBehaviour => m_RhythmPlayableBehaviour;
        public RhythmClipParameters ClipParameters => m_ClipParameters;
    
        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            //Debug.Log("Create Playable Clip");
            m_RhythmPlayableBehaviour.RhythmClip = this;
            var playable = ScriptPlayable<RhythmBehaviour>.Create (graph, m_RhythmPlayableBehaviour);
            return playable;
        }

        public virtual void Copy(RhythmClip otherClip)
        {
            return;
        }
    }

    [Serializable]
    public struct RhythmClipData
    {
        private RhythmDirector m_RhythmDirector;
        private int m_TrackID;
        private double m_ClipStart;
        private double m_RealDuration;
        private RhythmClipParameters m_ClipParameters;
        private RhythmClip m_RhythmClip;

        public RhythmDirector RhythmDirector=> m_RhythmDirector;
        public int TrackID => m_TrackID;
        public TrackObject TrackObject => m_RhythmDirector.TrackObjects[m_TrackID];
        public double ClipStart => m_ClipStart;
        public double ClipEnd => m_ClipStart + m_RealDuration;
        public double RealDuration => m_RealDuration;
        public RhythmClipParameters ClipParameters => m_ClipParameters;
        public RhythmClip RhythmClip => m_RhythmClip;

        public RhythmClipData(RhythmClip rhythmClip, RhythmDirector rhythmDirector, int trackID, double clipStart, double realDuration)
        {
            m_RhythmClip = rhythmClip;
            m_RhythmDirector = rhythmDirector;
            m_TrackID = trackID;
            m_ClipStart = clipStart;
            m_RealDuration = realDuration;
            m_ClipParameters = m_RhythmClip.ClipParameters;
        }
    }

    [Serializable]
    public class RhythmClipParameters
    {
        [Tooltip("The integer parameter.")]
        [SerializeField] protected int m_IntParameter;
        [Tooltip("The string parameter.")]
        [SerializeField] protected string m_StringParameter;
        [Tooltip("The float parameter.")]
        [SerializeField] protected float m_FloatParameter;
        [Tooltip("The Vector2 parameter.")]
        [SerializeField] protected Vector2 m_Vector2Parameter;
        [Tooltip("The Vector3 parameter.")]
        [SerializeField] protected Vector3 m_Vector3Parameter;
        [Tooltip("The Vector4 parameter.")]
        [SerializeField] protected Vector4 m_Vector4Parameter;
        [Tooltip("The Color parameter.")]
        [SerializeField] protected Color m_ColorParameter;
        [Tooltip("The Object parameter.")]
        [SerializeField] protected Object m_ObjectReferenceParameter;
        [Tooltip("The Object Array parameters.")]
        [SerializeField] protected Object[] m_ObjectReferenceArrayParameter;
    
        public int IntParameter => m_IntParameter;
        public string StringParameter => m_StringParameter;
        public float FloatParameter => m_FloatParameter;
        public Vector2 Vector2Parameter => m_Vector2Parameter;
        public Vector3 Vector3Parameter => m_Vector3Parameter;
        public Vector3 Vector4Parameter => m_Vector4Parameter;
        public Color ColorParameter => m_ColorParameter;
        public Object ObjectReferenceParameter => m_ObjectReferenceParameter;
        public Object[] ObjectReferenceArrayParameter => m_ObjectReferenceArrayParameter;
    }
}