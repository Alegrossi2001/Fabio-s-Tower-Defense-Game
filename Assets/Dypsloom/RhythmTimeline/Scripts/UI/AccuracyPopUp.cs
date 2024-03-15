/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.UI
{
    using Dypsloom.Shared.Utility;
    using System.Collections;
    using UnityEngine;

    public class AccuracyPopUp : MonoBehaviour
    {
        [Tooltip("The spawn position offset.")]
        [SerializeField] protected Vector3 m_SpawnPosOffset;
        [Tooltip("The pop up life time.")]
        [SerializeField] protected float m_LifeTime = 0.5f;

        protected Animator m_Animator;

        protected int m_AnimPop = Animator.StringToHash("Pop");

        protected WaitForSeconds m_LifeTimeWait;
        // Start is called before the first frame update
        protected virtual void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_LifeTimeWait = new WaitForSeconds(m_LifeTime);
        }

        public void OnEnable()
        {
            m_Animator.SetTrigger(m_AnimPop);
            StartCoroutine(PopIE());
        }
    
        private IEnumerator PopIE()
        {
            yield return null;
            transform.position += m_SpawnPosOffset;
            yield return m_LifeTimeWait;
            PoolManager.Destroy(gameObject);
        }
    }
}
