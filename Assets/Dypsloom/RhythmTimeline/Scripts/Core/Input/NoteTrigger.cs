namespace Dypsloom.RhythmTimeline.Core.Input
{
    using System;
    using Dypsloom.RhythmTimeline.Core.Notes;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Note Trigger allows you to trigger notes independently from TracksObjects
    /// Add it directly on the note object.
    /// This can be used to trigger notes through collisions, clicks, etc... 
    /// </summary>
    public class NoteTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Tooltip("The note linked to this component.")]
        [SerializeField] protected Note m_Note;
        [Tooltip("Trigger Note on Click (Requires Collider 2D or 3D)")]
        [SerializeField] protected bool m_UseOnPointerEvents = true;
        [Tooltip("Trigger Note On Trigger 2D Enter/Exit")]
        [SerializeField] protected bool m_UseOnTriggerEvents2D = true;
        [Tooltip("Trigger Note On Trigger 3D Enter/Exit")]
        [SerializeField] protected bool m_UseOnTriggerEvents3D = true;
        [Tooltip("Trigger Note On Collision 2D Enter/Exit")]
        [SerializeField] protected bool m_UseOnCollisionEvents2D = true;
        [Tooltip("Trigger Note On Collision 3D Enter/Exit")]
        [SerializeField] protected bool m_UseOnCollisionEvents3D = true;
        [Tooltip("The layer mask for trigger and collision events")]
        [SerializeField] protected LayerMask m_LayerMask = -1;

        protected InputEventData m_CachedInputEventData = new InputEventData(-1,-1);
        
        protected void Awake()
        {
            if (m_Note == null) {
                m_Note = GetComponent<Note>();
            }
        }
        
        public void TriggerStart()
        {
            m_CachedInputEventData.InputID = 0;
            Trigger(m_CachedInputEventData);
        }
        
        public void TriggerStop()
        {
            m_CachedInputEventData.InputID = 1;
            Trigger(m_CachedInputEventData);
        }

        public void Trigger(InputEventData noteTriggerEventData)
        {
            m_Note.OnTriggerInput(noteTriggerEventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_UseOnPointerEvents == false) {
                return;
            }
            TriggerStart();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_UseOnPointerEvents == false) {
                return;
            }
            TriggerStop();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (m_UseOnTriggerEvents3D == false) {
                return;
            }

            if (LayerContains(other.gameObject.layer) == false) {
                return;
            }
            TriggerStart();
        }
        
        public void OnTriggerExit(Collider other)
        {
            if (m_UseOnTriggerEvents3D == false) {
                return;
            }
            if (LayerContains(other.gameObject.layer) == false) {
                return;
            }
            TriggerStop();
        }
        
        public void OnTriggerEnter2D(Collider2D other)
        {
            if (m_UseOnTriggerEvents2D == false) {
                return;
            }
            if (LayerContains(other.gameObject.layer) == false) {
                return;
            }
            TriggerStart();
        }
        
        public void OnTriggerExit2D(Collider2D other)
        {
            if (m_UseOnTriggerEvents2D == false) {
                return;
            }
            if (LayerContains(other.gameObject.layer) == false) {
                return;
            }
            TriggerStop();
        }
        
        public void OnCollisionEnter(Collision other)
        {
            if (m_UseOnCollisionEvents3D == false) {
                return;
            }
            if (LayerContains(other.gameObject.layer) == false) {
                return;
            }
            TriggerStart();
        }
        
        public void OnCollisionExit(Collision other)
        {
            if (m_UseOnCollisionEvents3D == false) {
                return;
            }
            if (LayerContains(other.gameObject.layer) == false) {
                return;
            }
            TriggerStop();
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            if (m_UseOnCollisionEvents2D == false) {
                return;
            }
            if (LayerContains(other.gameObject.layer) == false) {
                return;
            }
            TriggerStart();
        }
        
        public void OnCollisionExit2D(Collision2D other)
        {
            if (m_UseOnCollisionEvents2D == false) {
                return;
            }
            if (LayerContains(other.gameObject.layer) == false) {
                return;
            }
            TriggerStop();
        }
        
        /// <summary>
        /// Extension method to check if a layer is in a layermask
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool LayerContains(int layer)
        {
            var mask = m_LayerMask;
            return mask == (mask | (1 << layer));
        }
        
    }
}