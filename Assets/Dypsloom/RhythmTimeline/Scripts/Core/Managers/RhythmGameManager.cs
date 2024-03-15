/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Managers
{
    using Dypsloom.RhythmTimeline.Scoring;
    using Dypsloom.RhythmTimeline.UI;
    using Dypsloom.Shared;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class RhythmGameManager : MonoBehaviour
    {
        [Tooltip("All The Rhythm Timeline Songs.")]
        [SerializeField] protected RhythmTimelineAsset[] m_Songs;
        [Tooltip("The Gameplay Panel.")]
        [SerializeField] protected GameObject m_GameplayPanel;
        [Tooltip("The UI Song Chooser.")]
        [SerializeField] protected SongChooserPanel m_SongChooser;
        [Tooltip("The End Score Panel")]
        [SerializeField] protected EndScorePanel m_EndScorePanel;
        [Tooltip("The Pause Menu.")]
        [SerializeField] protected GameObject m_PauseMenu;
        [Tooltip("The Welcome Menu.")]
        [SerializeField] protected GameObject m_WelcomeMenu;
        [Tooltip("The Rhythm Director.")]
        [SerializeField] protected RhythmDirector m_RhythmDirector;
        [Tooltip("The Timer UI.")]
        [SerializeField] protected TextMeshProUGUI m_TimerTmp;
        [Tooltip("The Pause Button.")]
        [SerializeField] protected Button m_PauseButton;
        [Tooltip("The Resume Button.")]
        [SerializeField] protected Button m_ResumeButton;
        [Tooltip("The Restart Button.")]
        [SerializeField] protected Button m_RestartButton;
        [Tooltip("The End song Button.")]
        [SerializeField] protected Button m_EndSongButton;
        [Tooltip("The Quit Button.")]
        [SerializeField] protected Button m_QuitButton;

    
        public RhythmTimelineAsset[] Songs => m_Songs;
        public SongChooserPanel SongChooser => m_SongChooser;
        public EndScorePanel EndScorePanel => m_EndScorePanel;
        public RhythmDirector RhythmDirector => m_RhythmDirector;
        public RhythmTimelineAsset SelectedSong => m_SelectedSong;

        protected bool m_IsPlaying;
        protected double m_StartTime;
        protected RhythmTimelineAsset m_SelectedSong;
        protected bool m_Paused;
        protected bool m_Restarting;

        private void Awake()
        {
            //Set this manager in the toolbox such that it may be found by other scripts easily.
            Toolbox.Set(this);
        }

        private void Start()
        {
            OpenSongChooser();
            m_RhythmDirector.OnSongEnd += OnSongEnd;
            if (m_PauseButton != null) {
                m_PauseButton.onClick.AddListener(TogglePause);
            }
            if (m_ResumeButton != null) {
                m_ResumeButton.onClick.AddListener(UnPause);
            }
            if (m_EndSongButton != null) {
                m_EndSongButton.onClick.AddListener(EndSong);
            }
            if (m_RestartButton != null) {
                m_RestartButton.onClick.AddListener(RestartSong);
            }
            if (m_QuitButton != null) {
                m_QuitButton.onClick.AddListener(QuitGame);
            }

            if (m_WelcomeMenu != null) {
                m_WelcomeMenu.SetActive(true);
            }
        }

        private void OpenSongChooser()
        {
            m_SongChooser.Open(this);
            m_GameplayPanel?.SetActive(false);
        }

        public void PlaySong(int index)
        {
            PlaySong(m_Songs[index]);
        }

        public void PlaySong(RhythmTimelineAsset song)
        {
            m_GameplayPanel?.SetActive(true);
            m_SelectedSong = song;
            m_RhythmDirector.PlaySong(song);
            StartTimer();
        }
    
        public void OnSongEnd()
        {
            m_IsPlaying = false;
            if(Application.isPlaying == false){ return; }
            
            // If we are restarting we don't want to save the score.
            if(m_Restarting){ return; }

            if (m_GameplayPanel != null) {
                m_GameplayPanel.SetActive(false);
            }

            var scoreManager = Toolbox.Get<ScoreManager>();
            if (scoreManager != null) {
                scoreManager.OnSongEnd(SelectedSong);
            }

            if (m_EndScorePanel != null) {
                m_EndScorePanel.Open(this);
            }
            
            DrawTimer(0);
        }

        protected virtual void DrawTimer(double timer)
        {
            m_TimerTmp.text = TimeSpan.FromSeconds(timer).ToString("mm':'ss'.'fff");
        }


        private void StartTimer()
        {
            m_StartTime = DspTime.AdaptiveTime;
            m_IsPlaying = true;
        }

        public void TogglePause()
        {
            if (m_Paused) {
                UnPause();
            } else {
                Pause();
            }
        }

        public void Pause()
        {
            m_PauseMenu.SetActive(true);
            m_Paused = true;
            Time.timeScale = 0;
            AudioListener.pause = true;
            m_RhythmDirector.Pause();
        }
    
        public void UnPause()
        {
            m_PauseMenu.SetActive(false);
            m_Paused = false;
            Time.timeScale = 1;
            AudioListener.pause = false;
            m_RhythmDirector.UnPause();
        }
    
        public void EndSong()
        {
            m_RhythmDirector.EndSong();
            UnPause();
        }
        
        public void RestartSong()
        {
            m_Restarting = true;
            
            var currentSong = m_SelectedSong;
            EndSong();
            PlaySong(currentSong);
            
            m_Restarting = false;
        }
        
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void Update()
        {
            if (m_IsPlaying && !m_Paused) {
                var timer = DspTime.AdaptiveTime - m_StartTime;
                DrawTimer(timer);
            }
        }

        private void OnDestroy()
        {
            m_RhythmDirector.OnSongEnd -=OnSongEnd;
            if (m_PauseButton != null) {
                m_PauseButton.onClick.RemoveListener(TogglePause);
            }
            if (m_ResumeButton != null) {
                m_ResumeButton.onClick.RemoveListener(UnPause);
            }
            if (m_EndSongButton != null) {
                m_EndSongButton.onClick.RemoveListener(EndSong);
            }
            if (m_RestartButton != null) {
                m_RestartButton.onClick.RemoveListener(RestartSong);
            }
            if (m_QuitButton != null) {
                m_QuitButton.onClick.RemoveListener(QuitGame);
            }
        }
    }
}
