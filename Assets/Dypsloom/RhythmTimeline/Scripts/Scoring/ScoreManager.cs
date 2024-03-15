/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Scoring
{
    using Dypsloom.RhythmTimeline.Core;
    using Dypsloom.RhythmTimeline.Core.Managers;
    using Dypsloom.RhythmTimeline.Core.Notes;
    using Dypsloom.RhythmTimeline.UI;
    using Dypsloom.Shared;
    using Dypsloom.Shared.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ScoreManager : MonoBehaviour
    {
        public event Action<RhythmTimelineAsset> OnNewHighScore;
        public event Action<Note, NoteAccuracy> OnNoteScore;
        public event Action OnBreakChain;
        public event Action<int> OnContinueChain;
        public event Action<float> OnScoreChange;

        [Tooltip("The Rhythm Director.")]
        [SerializeField] protected RhythmDirector m_RhythmDirector;
        [Tooltip("The score settings.")]
        [SerializeField] protected ScoreSettings m_ScoreSettings;
        [Tooltip("The score text.")]
        [SerializeField] protected TMPro.TextMeshProUGUI m_ScoreTmp;
        [Tooltip("The score multiplier text.")]
        [SerializeField] protected TMPro.TextMeshProUGUI m_ScoreMultiplierTmp;
        [Tooltip("The chain text.")]
        [SerializeField] protected TMPro.TextMeshProUGUI m_ChainTmp;
        [Tooltip("The rank slider.")]
        [SerializeField] protected RankSlider m_RankSlider;
        [Tooltip("(Optional) the spawn point for the accuracy pop ups.")]
        [SerializeField] protected Transform m_AccuracySpawnPoint;
    
        protected RhythmTimelineAsset m_CurrentSong;
        protected float m_CurrentScore;
        protected List<int> m_CurrentAccuracyIDHistogram;
        protected int m_CurrentMaxChain;
        protected int m_CurrentChain;
        protected float m_CurrentMaxPossibleScore;

        protected float m_ScoreMultiplier = 1;
        protected Coroutine m_ScoreMultiplierCoroutine;

        private static ScoreManager m_Instance;

        public static ScoreManager Instance => m_Instance;
    
        public ScoreSettings ScoreSettings => m_ScoreSettings;
        public RhythmTimelineAsset CurrentSong => m_CurrentSong;

        protected void Awake()
        {
            m_Instance = this;
            Toolbox.Set(this);
        }

        private void Start()
        {
            if (m_RhythmDirector == null) {
                m_RhythmDirector = Toolbox.Get<RhythmDirector>();
            }
            
            UpdateScoreVisual();
            m_RhythmDirector.OnSongPlay += HandleOnSongPlay;
            m_RhythmDirector.OnSongEnd += HandleOnSongEnd;
            m_RhythmDirector.RhythmProcessor.OnNoteTriggerEvent += HandleOnNoteTriggerEvent;

            if (m_RhythmDirector.IsPlaying) {
                HandleOnSongPlay();
            }
        }

        private void HandleOnNoteTriggerEvent(NoteTriggerEventData noteTriggerEventData)
        {
            var noteAccuracy = AddNoteAccuracyScore(
                noteTriggerEventData.Note,
                noteTriggerEventData.DspTimeDifferencePercentage,
                noteTriggerEventData.Miss);

            var spawnTransform = m_AccuracySpawnPoint != null
                ? m_AccuracySpawnPoint
                : noteTriggerEventData.Note.RhythmClipData.TrackObject.EndPoint;
           
            ScorePopup(spawnTransform, noteAccuracy);
        }

        private void HandleOnSongEnd()
        {
            OnSongEnd(m_RhythmDirector.SongTimelineAsset);
        }

        private void HandleOnSongPlay()
        {
            SetSong(m_RhythmDirector.SongTimelineAsset);
        }

        public void SetSong(RhythmTimelineAsset song)
        {
            m_CurrentSong = song;
            m_CurrentScore = m_CurrentSong.StartScore;

            m_CurrentMaxPossibleScore = m_CurrentSong.MaxScore;
            if (m_CurrentMaxPossibleScore < 0) {
                m_CurrentMaxPossibleScore = song.RhythmClipCount * m_ScoreSettings.MaxNoteScore;
            }
            
            m_CurrentAccuracyIDHistogram = new List<int>(m_CurrentSong.RhythmClipCount);
            m_CurrentMaxChain = 0;
            m_CurrentChain = 0;

            m_ScoreMultiplier = 1;
        
            UpdateScoreVisual();
        }

        public void OnSongEnd(RhythmTimelineAsset song)
        {
            if (m_CurrentAccuracyIDHistogram == null) {
                SetSong(song);
            }
            
            var newScore = new ScoreData(m_CurrentAccuracyIDHistogram.ToArray(), m_CurrentScore, m_CurrentMaxChain,
                m_ScoreSettings, song);

            if (newScore.FullScore > song.HighScore.FullScore) {
                song.SetHighScore(newScore);
                OnNewHighScore?.Invoke(song);
            }
        }

        public ScoreData GetScoreData()
        {
            return new ScoreData(m_CurrentAccuracyIDHistogram.ToArray(),m_CurrentScore,m_CurrentMaxChain,m_ScoreSettings,m_CurrentSong);
        }

        public NoteAccuracy GetAccuracy(float offsetPercentage, bool miss)
        {
            if (miss) { return GetMissAccuracy(); }

            return GetAccuracy(offsetPercentage);
        }
        
        public NoteAccuracy GetAccuracy(float offsetPercentage)
        {
            var noteAccuracy = m_ScoreSettings.GetNoteAccuracy(offsetPercentage);
            
            if (noteAccuracy == null) {
                Debug.LogWarningFormat("Note Accuracy could not be found for offset ({0}), make sure the score settings are correctly set up", offsetPercentage);
                return null;
            }

            return noteAccuracy;
        }
        
        public NoteAccuracy GetMissAccuracy()
        {
            return  m_ScoreSettings.GetMissAccuracy();
        }
        
        public virtual NoteAccuracy AddNoteAccuracyScore(Note note, float offsetPercentage, bool miss)
        {
            NoteAccuracy noteAccuracy = GetAccuracy(offsetPercentage, miss);

            AddNoteAccuracyScore(note, noteAccuracy);

            return noteAccuracy;
        }

        public virtual void AddNoteAccuracyScore(Note note, NoteAccuracy noteAccuracy)
        {
            
            if (noteAccuracy == null) {
                Debug.LogWarningFormat("Note Accuracy is null");
                return;
            }

            //Chain
            if (noteAccuracy.breakChain) {
                m_CurrentChain = 0;
                OnBreakChain?.Invoke();
            } else {
                m_CurrentChain++;
                m_CurrentMaxChain = Mathf.Max(m_CurrentChain, m_CurrentMaxChain);
                OnContinueChain?.Invoke(m_CurrentChain);
            }
            
            AddScore(noteAccuracy.score);
            OnNoteScore?.Invoke(note,noteAccuracy);

            if (m_CurrentSong == null) { return; }

            m_CurrentAccuracyIDHistogram.Add(m_ScoreSettings.GetID(noteAccuracy));
        }

        public virtual void AddScore(float score)
        {
            if (score < 0) {
                m_CurrentScore += score;
            } else {
                m_CurrentScore += m_ScoreMultiplier*score;
            }

            if (m_CurrentSong.PreventMaxScoreOvershoot) {
                m_CurrentScore = Mathf.Min(m_CurrentScore, m_CurrentMaxPossibleScore);
            }
            
            if (m_CurrentSong.PreventMinScoreOvershoot) {
                m_CurrentScore = Mathf.Max(m_CurrentScore, m_CurrentSong.MinScore);
            }
            
            OnScoreChange?.Invoke(m_CurrentScore);
            UpdateScoreVisual();
        }

        public void SetMultiplier(float multiplier)
        {
            m_ScoreMultiplier = multiplier;
            UpdateScoreVisual();
        }
        
        public void SetMultiplier(float multiplier,float time)
        {
            SetMultiplier(multiplier);

            if (m_ScoreMultiplierCoroutine != null) { StopCoroutine(m_ScoreMultiplierCoroutine); }
            m_ScoreMultiplierCoroutine = StartCoroutine(ResetMultiplierDelayed(time));
        }

        public IEnumerator ResetMultiplierDelayed(float delay)
        {
            var start = DspTime.AdaptiveTime;
            while (start + delay > DspTime.AdaptiveTime) { yield return null; }
            SetMultiplier(1);
        }
        
        public int GetChain()
        {
            return m_CurrentChain;
        }
        
        public float GetChainPercentage()
        {
            var maxScore = m_CurrentSong.RhythmClipCount;
            var percentage = 100 * m_CurrentChain / maxScore;
            return percentage;
        }
        
        public float GetMaxChain()
        {
            return m_CurrentMaxChain;
        }
        
        public float GetMaxChainPercentage()
        {
            var maxScore = m_CurrentSong.RhythmClipCount;
            var percentage = 100 * m_CurrentMaxChain / maxScore;
            return percentage;
        }
        
        public float GetScore()
        {
            return m_CurrentScore;
        }

        public float GetScorePercentage()
        {
            var percentage = m_CurrentScore * 100 / m_CurrentMaxPossibleScore;
            return percentage;
        }
        
        public ScoreRank GetRank()
        {
            return m_ScoreSettings.GetRank(GetScorePercentage());
        }

        public void UpdateScoreVisual()
        {
            if (m_ScoreTmp != null) {
                m_ScoreTmp.text = m_CurrentScore.ToString();
            }
            if (m_ScoreMultiplierTmp != null) {
                m_ScoreMultiplierTmp.text = m_ScoreMultiplier == 1 ? "" : $"X{m_ScoreMultiplier}";
            }
            if (m_ChainTmp != null) {
                m_ChainTmp.text = m_CurrentChain.ToString();
            }

            if (m_RankSlider != null) {
                if (m_CurrentSong == null) {
                    m_RankSlider.SetRank(0, m_ScoreSettings.GetRank(0));
                } else {
                    var percentage =  GetScorePercentage();
                    m_RankSlider.SetRank(percentage, m_ScoreSettings.GetRank(percentage));
                }
            }
        }

        public virtual void ScorePopup(Transform spawnPoint, NoteAccuracy noteAccuracy)
        {
            Pop(noteAccuracy.popPrefab, spawnPoint);
        }

        public virtual void Pop(GameObject prefab, Transform spawnPoint)
        {
            PoolManager.Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
