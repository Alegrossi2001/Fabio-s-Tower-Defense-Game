/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.UI
{
   using Dypsloom.RhythmTimeline.Core;
   using Dypsloom.RhythmTimeline.Core.Managers;
   using Dypsloom.RhythmTimeline.Scoring;
   using System;
   using TMPro;
   using UnityEngine;
   using UnityEngine.UI;

   public class SelectedSongPanel : MonoBehaviour
   {
      [Tooltip("The title text.")]
      [SerializeField] protected TextMeshProUGUI m_TitleTmp;
      [Tooltip("The artist text.")]
      [SerializeField] protected TextMeshProUGUI m_ArtistTmp;
      [Tooltip("The song length in time text.")]
      [SerializeField] protected TextMeshProUGUI m_TimeTmp;
      [Tooltip("The high score.")]
      [SerializeField] protected HighScoreUI m_HighScoreUi;
      [Tooltip("The difficulty text.")]
      [SerializeField] protected TextMeshProUGUI m_Difficulty;
      [Tooltip("The play song button.")]
      [SerializeField] protected Button m_PlaySongButton;
      [Tooltip("The audio source to play the song audio clip preview.")]
      [SerializeField] protected AudioSource m_AudioSource;
      [Tooltip("The song clip preview length.")]
      [SerializeField] protected float m_PreviewLength = 15;

      protected SongChooserPanel m_SongChooserPanel;
      protected RhythmTimelineAsset m_SelectedSong;
   
      private void Awake()
      {
         m_PlaySongButton.onClick.AddListener(PlaySelectedSong);
      }

      private void PlaySelectedSong()
      {
         m_SongChooserPanel.PlaySelectedSong();
      }

      public void Open(SongChooserPanel songChooserPanel)
      {
         m_SongChooserPanel = songChooserPanel;
      }

      public void SetSelectedSong(RhythmTimelineAsset song)
      {
         m_SelectedSong = song;
         if (m_SelectedSong.HighScore == null) {
            m_SelectedSong.SetHighScore(new ScoreData());
         }
         m_SelectedSong.HighScore.Initialize(ScoreManager.Instance.ScoreSettings,m_SelectedSong);
      
         UpdateVisual();
      
         m_AudioSource.clip = m_SelectedSong.AudioClip;
         PlayPreviewClip();
      }

      private void UpdateVisual()
      {
         m_TitleTmp.text = m_SelectedSong.FullName;
         m_ArtistTmp.text = m_SelectedSong.Authour;
         m_TimeTmp.text = TimeSpan.FromSeconds(m_SelectedSong.duration).ToString("mm':'ss'.'fff");
         m_HighScoreUi.SetScoreData(m_SelectedSong.HighScore);
         m_Difficulty.text = $"{m_SelectedSong.Dificulty}";
      }

      public void PlayPreviewClip()
      {
         m_AudioSource.Play(0);
      }

      public void StopPreviewClip()
      {
         m_AudioSource.Stop();
      }

      private void Update()
      {
         if (m_AudioSource.isPlaying) {
            if (m_AudioSource.time > m_PreviewLength) {
               m_AudioSource.Play(0);
            }
         }
      }
   }
}
