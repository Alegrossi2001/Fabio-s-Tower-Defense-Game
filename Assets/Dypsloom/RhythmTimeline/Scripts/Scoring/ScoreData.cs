/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Scoring
{
    using Dypsloom.RhythmTimeline.Core;
    using Dypsloom.RhythmTimeline.Core.Managers;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class ScoreData
    {
        [Tooltip("The note accuracy histogram.")]
        [SerializeField] protected int[] m_NoteAccuracyIDHistogram;
        [Tooltip("The full score.")]
        [SerializeField] protected float m_FullScore;
        [Tooltip("The max chain.")]
        [SerializeField] protected int m_MaxChain;
        
        [Tooltip("The note accuracy ID count.")]
        [NonSerialized] protected int[] m_NoteAccuracyIDCounts;
        [Tooltip("Was the score a full chain?")]
        [NonSerialized] protected bool m_FullChain;
        [Tooltip("The rank.")]
        [NonSerialized] protected ScoreRank m_Rank;

        public IReadOnlyList<int> NoteAccuracyIDCounts => m_NoteAccuracyIDCounts;
        public IReadOnlyList<int> NoteAccuracyIDHistogram => m_NoteAccuracyIDHistogram;
        public float FullScore => m_FullScore;
        public int MaxChain => m_MaxChain;
        public bool FullChain => m_FullChain;
        public ScoreRank Rank => m_Rank;

        public ScoreData()
        { }
        
        public ScoreData(int[] histogram, float fullScore, int maxChain, ScoreSettings scoreSettings, RhythmTimelineAsset song)
        {
            m_NoteAccuracyIDHistogram = histogram;
            m_FullScore = fullScore;
            m_MaxChain = maxChain;
            Initialize(scoreSettings,song);
        }

        public void Initialize(ScoreSettings scoreSettings, RhythmTimelineAsset song)
        {
            //Note compute Accuracy ID Counts from histogram
            var accuracyTable = scoreSettings.OrderedAccuracyTable;
            m_NoteAccuracyIDCounts = new int[accuracyTable.Count];
            for (int i = 0; i < m_NoteAccuracyIDHistogram.Length; i++) {
                var noteAccuracyID = m_NoteAccuracyIDHistogram[i];

                if (noteAccuracyID < 0 || noteAccuracyID >= m_NoteAccuracyIDCounts.Length) {
                    //This can happen if the note accuracy score settings has changed, causing saved note accuracy to be out of range.
                    continue;
                }
                
                m_NoteAccuracyIDCounts[noteAccuracyID]++;
            }
        
            //Check if fullChain
            m_FullChain = m_MaxChain >= song.RhythmClipCount;

            //Compute Rank
            var maxPossibleScore = 0f;
            if (song.MaxScore < 0) {
                maxPossibleScore = song.RhythmClipCount * accuracyTable[0].score;
            } else {
                //Compute Rank
                maxPossibleScore = song.MaxScore;
            }

            if (song.PreventMaxScoreOvershoot) {
                m_FullScore = Mathf.Min(m_FullScore, maxPossibleScore);
            }
            
            if (song.PreventMinScoreOvershoot) {
                m_FullScore = Mathf.Max(m_FullScore, song.MinScore);
            }

            var percentage = 100f * m_FullScore / maxPossibleScore;
            m_Rank = scoreSettings.GetRank(percentage);
        }
    }
}
