/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core
{
    using Dypsloom.Shared;
    using UnityEngine;

    public class DspTime : MonoBehaviour
    {
        private double m_Time;
        private double m_AdaptiveTime;

        public static double Time
        {
            get
            {
                var dspTime = Toolbox.Get<DspTime>();
                return dspTime == null ? AudioSettings.dspTime : dspTime.m_Time;
            }
        }

        public static double AdaptiveTime  {
            get
            {
                var dspTime = Toolbox.Get<DspTime>();
                return dspTime == null ? AudioSettings.dspTime : dspTime.m_AdaptiveTime;
            }
        }
        
        // Protected Prevent non-singleton constructor use.
        protected DspTime()
        {
            m_Time = AudioSettings.dspTime;
            m_AdaptiveTime = m_Time;
        }
        
        private void Awake()
        {
            m_Time = AudioSettings.dspTime;
            m_AdaptiveTime = m_Time;
        }

        private void Start()
        {
            m_Time = AudioSettings.dspTime;
            m_AdaptiveTime = m_Time;
            
            Toolbox.Set<DspTime>(this);
        }

        private void Update()
        {
            if (m_Time == AudioSettings.dspTime) {
                m_AdaptiveTime += UnityEngine.Time.unscaledDeltaTime;
            } else {
                m_Time = AudioSettings.dspTime;
                m_AdaptiveTime = m_Time;
            }
        }
    }
}