/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.UI
{
    using Dypsloom.RhythmTimeline.Core;
    using Dypsloom.RhythmTimeline.Core.Managers;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SongButton : MonoBehaviour
    {
        [Tooltip("The song name text.")]
        [SerializeField] protected TextMeshProUGUI m_SongName;
        [Tooltip("The button to select the song.")]
        [SerializeField] protected Button m_Button;


        protected RhythmTimelineAsset m_Song;
        protected int m_Index;

        public RhythmTimelineAsset Song => m_Song;
        public int Index => m_Index;

        public Button Button => m_Button;
        public TextMeshProUGUI SongName => m_SongName;

        public void Initialize(RhythmTimelineAsset song, int index, Action<SongButton> onClickAction)
        {
            m_Song = song;
            m_Index = index;

            m_SongName.text = m_Song.FullName;
            m_Button.onClick.AddListener(() => onClickAction.Invoke(this));
        }
    }
}
