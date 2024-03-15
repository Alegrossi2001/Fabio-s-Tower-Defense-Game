/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.UI
{
    using Dypsloom.RhythmTimeline.Scoring;
    using UnityEngine;
    using UnityEngine.UI;

    public class RankSlider : MonoBehaviour
    {
        [Tooltip("The slider.")]
        [SerializeField] protected Slider m_Slider;
        [Tooltip("The rank image.")]
        [SerializeField] protected Image m_RankImage;
        [Tooltip("The image slider fill (optional).")]
        [SerializeField] protected Image m_SliderFill;
        [Tooltip("The Slider Gradient.")]
        [SerializeField] protected Gradient m_SliderGradient;

        public void SetRank(float p, ScoreRank rank)
        {
            if (m_Slider != null) {
                m_Slider.value = p;
            }

            if (m_SliderFill != null && m_SliderGradient !=null) {
                m_SliderFill.color = m_SliderGradient.Evaluate(p / 100f);
            }
            
            if(m_RankImage != null)
            {
                m_RankImage.sprite = rank?.icon;
            }
        }
    }
}
