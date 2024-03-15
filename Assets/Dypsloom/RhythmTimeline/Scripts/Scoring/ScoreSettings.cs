/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Scoring
{
    using Dypsloom.RhythmTimeline.Core.Managers;
    using Dypsloom.Shared;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Serialization;

    [System.Serializable]
    public class NoteAccuracy
    {
        public string name;
        public bool miss;
        public bool breakChain;
        public float percentageTheshold;
        public float score;
        public Sprite icon;
        public GameObject popPrefab;
    }

    [System.Serializable]
    public class ScoreRank
    {
        public string name;
        public Sprite icon;
        public float percentageTheshold;
    }

    [CreateAssetMenu(fileName = "My Score Setting", menuName = "Dypsloom/Rhythm Timeline/Score Setting", order = 1)]
    public class ScoreSettings : ScriptableObject
    {
        [Tooltip("The accuracy table.")]
        [SerializeField] protected NoteAccuracy[] m_AccuracyTable;
        [Tooltip("The rank table.")]
        [SerializeField] protected ScoreRank[] m_RankTable;
    
        protected Dictionary<string, NoteAccuracy> m_AccuracyDictionary;
        protected Dictionary<string, ScoreRank> m_RankDictionary;

        protected float m_MaxNoteScore;

        public float MaxNoteScore => m_MaxNoteScore;

        public virtual IReadOnlyDictionary<string, NoteAccuracy> AccuracyDictionary
        {
            get
            {
                if (!m_Initialized) { Initialize(); }

                return m_AccuracyDictionary;
            }
        }
        public virtual IReadOnlyList<NoteAccuracy> OrderedAccuracyTable
        {
            get
            {
                if (!m_Initialized) { Initialize(); }

                return m_AccuracyTable;
            }
        }
        public virtual IReadOnlyDictionary<string, ScoreRank> RankDictionary
        {
            get
            {
                if (!m_Initialized) { Initialize(); }

                return m_RankDictionary;
            }
        }
        public virtual IReadOnlyList<ScoreRank> OrderedRankTable
        {
            get
            {
                if (!m_Initialized) { Initialize(); }

                return m_RankTable;
            }
        }

        [NonSerialized] protected bool m_Initialized = false;
    
        public void Initialize()
        {
            //Note Accuracy
            Array.Sort(m_AccuracyTable, new Comparison<NoteAccuracy>( 
                (x, y) => x.miss ? 1 : y.miss ? -1 : x.percentageTheshold.CompareTo(y.percentageTheshold))); 
        
            m_AccuracyDictionary = new Dictionary<string, NoteAccuracy>();
            for (int i = 0; i < m_AccuracyTable.Length; i++) {
                m_AccuracyDictionary.Add(m_AccuracyTable[i].name, m_AccuracyTable[i]);
            }
        
            //Rank
            Array.Sort(m_RankTable, new Comparison<ScoreRank>( 
                (x, y) => x.percentageTheshold.CompareTo(y.percentageTheshold))); 
        
            m_RankDictionary = new Dictionary<string, ScoreRank>();
            for (int i = 0; i < m_RankTable.Length; i++) {
                m_RankDictionary.Add(m_RankTable[i].name, m_RankTable[i]);
            }

            m_MaxNoteScore = GetNoteAccuracy(0).score;

            m_Initialized = true;
        }

        public virtual NoteAccuracy GetNoteAccuracy(float offsetPercentage)
        {
            NoteAccuracy last = null;
            
            for (int i = 0; i < m_AccuracyTable.Length; i++) {
                if (m_AccuracyTable[i].miss) { continue; }
                
                if (offsetPercentage <= m_AccuracyTable[i].percentageTheshold) {
                    //Debug.LogError(offsetPercentage +" <= " +m_AccuracyTable[i].percentageTheshold+" "+m_AccuracyTable[i].name);
                    return m_AccuracyTable[i];
                }
                
                last = m_AccuracyTable[i];
            }

            return last;
        }
    
        public virtual NoteAccuracy GetMissAccuracy()
        {
            for (int i = 0; i < m_AccuracyTable.Length; i++) {
                if (m_AccuracyTable[i].miss) { return m_AccuracyTable[i]; }
            }

            return null;
        }
    
        public virtual ScoreRank GetRank(float percentage)
        {
            ScoreRank scoreRank = m_RankTable[0];
            for (int i = 0; i < m_RankTable.Length; i++) {

                if (percentage > m_RankTable[i].percentageTheshold) {
                    scoreRank = m_RankTable[i]; 
                    continue;
                }
                break;
            }

            return scoreRank;
        }

        public virtual int GetID(NoteAccuracy noteAccuracy)
        {
            return Array.IndexOf(m_AccuracyTable, noteAccuracy);
        }
    }
}