/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Playables
{
    using UnityEngine;
    using UnityEngine.Timeline;

    [ExcludeFromPreset]
    [HideInMenu]
    public class TempoMarker : Marker
    {
        [Tooltip("The Tempro Marker ID, used to define on/off beat (and potentially others).")]
        [SerializeField] protected int m_ID;

        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public virtual void Copy(TempoMarker clonedFrom)
        {
            if(clonedFrom == null){return;}
            m_ID = clonedFrom.m_ID;
            return;
        }
    }
}