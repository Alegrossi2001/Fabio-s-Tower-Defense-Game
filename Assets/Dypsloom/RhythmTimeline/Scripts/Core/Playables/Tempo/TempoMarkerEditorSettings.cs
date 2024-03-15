/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

#if UNITY_EDITOR
namespace Dypsloom.RhythmTimeline.Core.Playables
{
    using System;
    using UnityEngine;

    [Serializable]
    [CreateAssetMenu(fileName = "TempoMarkerEditorSettings", menuName = "Dypsloom/Rhythm Timeline/Tempo Marker Editor Settings", order = 50)]
    public class TempoMarkerEditorSettings : ScriptableObject
    {
        [Tooltip("The the ID used to match with the Tempo mark IDs.")]
        [SerializeField] private int m_ID;
        [Tooltip("The Texture for normal.")]
        [SerializeField] private Texture m_Normal;
        [Tooltip("The texture when the marker is collapsed.")]
        [SerializeField] private Texture m_Collapsed;
        [Tooltip("The plus textre when two markers or more are close to each other.")]
        [SerializeField] private Texture m_Plus;
        [Tooltip("The default color.")]
        [SerializeField] private Color m_DefaultColor = Color.white;
        [Tooltip("The color when selected.")]
        [SerializeField] private Color m_SelectedColor = new Color(0.1f, 0.1f, 1f, 0.3f);

        public int ID
        {
            get => m_ID;
            set => m_ID = value;
        }

        public virtual Texture Normal
        {
            get => m_Normal;
            set => m_Normal = value;
        }

        public virtual Texture Collapsed
        {
            get => m_Collapsed;
            set => m_Collapsed = value;
        }

        public virtual Texture Plus
        {
            get => m_Plus;
            set => m_Plus = value;
        }

        public Color DefaultColor
        {
            get => m_DefaultColor;
            set => m_DefaultColor = value;
        }

        public Color SelectedColor
        {
            get => m_SelectedColor;
            set => m_SelectedColor = value;
        }
    }
}
#endif