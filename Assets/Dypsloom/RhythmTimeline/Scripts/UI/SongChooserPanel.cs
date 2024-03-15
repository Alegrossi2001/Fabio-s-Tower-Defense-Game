/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.UI
{
    using Dypsloom.RhythmTimeline.Core.Managers;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class SongChooserPanel : MonoBehaviour
    {
        [Tooltip("The song button prefab.")]
        [SerializeField] protected SongButton m_SongButtonPrefab;
        [Tooltip("The scroll bar.")]
        [SerializeField] protected ScrollRect m_ScrollRect;
        [Tooltip("The scroll view content.")]
        [SerializeField] protected RectTransform m_ScrollViewContent;
        [Tooltip("The selected song panel.")]
        [SerializeField] protected SelectedSongPanel m_SelectedSongPanel;

        protected List<SongButton> m_ButtonsList;
        protected RhythmGameManager m_RhythmGameManager;
        protected int m_SelectedIndex;
        protected bool m_IsOpen;
        protected bool m_Populated = false;
        protected float m_ViewHeight;
        protected float m_ButtonHeight;

        public int SelectedIndex => m_SelectedIndex;
        public bool IsOpen => m_IsOpen;

        public RhythmGameManager RhythmGameManager => m_RhythmGameManager;

        public void Open(RhythmGameManager gameManager)
        {
            m_IsOpen = true;
            m_RhythmGameManager = gameManager;
            PopulateScrollView();
            m_SelectedSongPanel.Open(this);
            m_SelectedSongPanel.SetSelectedSong(m_RhythmGameManager.Songs[m_SelectedIndex]);
            gameObject.SetActive(true);
        }

        public void PopulateScrollView()
        {
            if (m_Populated) { return;}
            m_ButtonsList = new List<SongButton>();
            m_Populated = true;

            m_ViewHeight = m_ScrollViewContent.rect.height;
            m_ButtonHeight = (m_SongButtonPrefab.transform as RectTransform)?.rect.height ?? 100;

            for (int i = 0; i < m_RhythmGameManager.Songs.Length; i++) {
                var songButton = GameObject.Instantiate(m_SongButtonPrefab, m_ScrollViewContent);
                songButton.Initialize(m_RhythmGameManager.Songs[i], i, OnSongButtonClick);
                m_ButtonsList.Add(songButton);
            }
        }

        private void OnSongButtonClick(SongButton songButton)
        {
            m_SelectedIndex = songButton.Index;
            m_SelectedSongPanel.SetSelectedSong(songButton.Song);
        }

        public void SetSelectedSong(int index)
        {
            m_SelectedIndex = index;
            if (index < 0 || index >= m_RhythmGameManager.Songs.Length) {
                m_SelectedIndex = 0;
            }
            
            m_ButtonsList[index].Button.Select();
            
            var verticalPosition = 1f - (float)m_SelectedIndex / (m_ButtonsList.Count - 1);
            m_ScrollRect.verticalNormalizedPosition = verticalPosition;

            m_SelectedSongPanel.SetSelectedSong(m_RhythmGameManager.Songs[index]);
        }

        public void Close()
        {
            m_IsOpen = false;
            gameObject.SetActive(false);
        }

        public void PlaySelectedSong()
        {
            m_SelectedSongPanel.StopPreviewClip();
            m_RhythmGameManager.PlaySong(m_RhythmGameManager.Songs[m_SelectedIndex]);
            Close();
        }
    }
}
