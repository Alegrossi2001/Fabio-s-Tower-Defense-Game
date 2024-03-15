/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.UI
{
    using Dypsloom.RhythmTimeline.Scoring;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class HighScoreUI : MonoBehaviour
    {
        [Tooltip("The score text.")]
        [SerializeField] protected TextMeshProUGUI m_ScoreTmp;
        [Tooltip("The rank image.")]
        [SerializeField] protected Image m_RankImage;
        [Tooltip("The max chain text.")]
        [SerializeField] protected TextMeshProUGUI m_MaxChainTmp;
        [Tooltip("The accuracy UI.")]
        [SerializeField] protected AccuracyCountUI[] m_AccuracyUi;

        public void SetScoreData(ScoreData scoreData)
        {
            m_ScoreTmp.text = scoreData.FullScore.ToString("n2");
            m_RankImage.sprite = scoreData.Rank?.icon;
            m_MaxChainTmp.text = scoreData.MaxChain.ToString();

            for (int i = 0; i < m_AccuracyUi.Length; i++) {
                m_AccuracyUi[i].SetAccuracyCount(ScoreManager.Instance.ScoreSettings.OrderedAccuracyTable[i].icon, scoreData.NoteAccuracyIDCounts[i]);
            }
        }
    }
}
