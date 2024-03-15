/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class AccuracyCountUI : MonoBehaviour
    {
        [Tooltip("The accuracy image.")]
        [SerializeField] protected Image m_AccuracyImage;
        [Tooltip("The counter text.")]
        [SerializeField] protected TextMeshProUGUI m_CountTmp;

        public void SetAccuracyCount(Sprite icon, int count)
        {
            m_AccuracyImage.sprite = icon;
            m_CountTmp.text = count.ToString();
        }
    }
}
