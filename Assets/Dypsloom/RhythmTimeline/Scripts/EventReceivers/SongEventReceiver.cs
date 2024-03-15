namespace Dypsloom.RhythmTimeline.Effects
{
    using Dypsloom.RhythmTimeline.Core.Managers;
    using Dypsloom.Shared;
    using UnityEngine;
    using UnityEngine.Events;

    public class SongEventReceiver : MonoBehaviour
    {
        [Tooltip("Event whe the song starts.")]
        [SerializeField] protected UnityEvent m_OnSongStart;
        [Tooltip("Event when the song ends.")]
        [SerializeField] protected UnityEvent m_OnSongEnd;
        
        protected RhythmDirector m_RhythmDirector;

        private void Start()
        {
            if (m_RhythmDirector == null) { m_RhythmDirector = Toolbox.Get<RhythmDirector>(); }

            m_RhythmDirector.OnSongPlay += HandleOnSongPlay;
            m_RhythmDirector.OnSongEnd += HandleOnSongEnd;
        }

        private void HandleOnSongPlay()
        {
            m_OnSongStart.Invoke();
        }
        
        private void HandleOnSongEnd()
        {
            m_OnSongEnd.Invoke();
        }
    }
}