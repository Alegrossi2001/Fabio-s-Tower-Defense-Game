/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.UI
{
    using Dypsloom.RhythmTimeline.Core.Managers;
    using Dypsloom.RhythmTimeline.Scoring;
    using Dypsloom.Shared;
    using UnityEngine;
    using UnityEngine.UI;

    public class EndScorePanel : MonoBehaviour
    {
        [Tooltip("The current score.")]
        [SerializeField] protected HighScoreUI m_ScoreUi;
        [Tooltip("The high score.")]
        [SerializeField] protected HighScoreUI m_HighScoreUi;
        [Tooltip("The button to proceed.")]
        [SerializeField] protected Button m_NextButton;
    
        protected RhythmGameManager m_RhythmGameManager;

        private void Awake()
        {
            m_NextButton.onClick.AddListener(Next);
        }

        public void Open(RhythmGameManager gameManager)
        {
            m_RhythmGameManager = gameManager;
            gameObject.SetActive(true);

            var scoreManager = Toolbox.Get<ScoreManager>();
            if (scoreManager == null) {
                Debug.LogWarning("Score Manager not found in Toolbox.",gameObject);
                return;
            }
            var newScore = scoreManager.GetScoreData();
            var highScore = m_RhythmGameManager.SelectedSong.HighScore;

            UpdateVisual(newScore,highScore);
        }

        private void UpdateVisual(ScoreData score, ScoreData highScore)
        {
            m_ScoreUi.SetScoreData(score);
            m_HighScoreUi.SetScoreData(highScore);
        }

        public void Next()
        {
            Close();
            m_RhythmGameManager.SongChooser.Open(m_RhythmGameManager);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
