namespace Dypsloom.RhythmTimeline.Core.Input
{
    using Dypsloom.RhythmTimeline.Core.Notes;
    using UnityEngine;

    /// <summary>
    /// Note Raycaster allows you to trigger notes independently from TracksObjects
    /// Add it with the managers in your scene.
    /// The notes must have a 2D or 3D collider to detect 
    /// </summary>
    public class NoteRaycaster : MonoBehaviour
    {
        [Tooltip("Camera to use for raycasting. (if null camera main will be used).")]
        [SerializeField] public Camera RaycastCamera;
        [Tooltip("Use the mouse button input up and down events.")]
        [SerializeField] protected bool m_UseMouseButtonDownUp;
        [Tooltip("Use 2D Physics.")]
        [SerializeField] protected bool m_UsePhysics2D;
        [Tooltip("Use 3D physics.")]
        [SerializeField] protected bool m_UsePhysics3D;
        [Tooltip("The layer mask for raycast.")]
        [SerializeField] protected LayerMask m_LayerMask = -1;
        
        
        protected InputEventData m_CachedInputEventData = new InputEventData(-1,-1);
        

        private void Awake()
        {
            if (RaycastCamera == null) {
                RaycastCamera = Camera.main;
            }
        }

        public void Update()
        {
            Note note = null;

            var inputUp = Input.GetMouseButtonUp(0);
            var inputDown = Input.GetMouseButtonDown(0);

            if (!m_UseMouseButtonDownUp || (!inputUp && !inputDown)) { return; }

            if ((!m_UsePhysics2D || !RaycastNote2D(Input.mousePosition, out note)) &&
                (!m_UsePhysics3D || !RaycastNote3D(Input.mousePosition, out note))) { return; }

            m_CachedInputEventData.InputID = inputUp ? 1 : 0;
            note.OnTriggerInput(m_CachedInputEventData);
        }

        public bool RaycastNote2D(Vector3 screenPosition, out Note note)
        {
            RaycastHit2D rayHit = Physics2D.GetRayIntersection(RaycastCamera.ScreenPointToRay(screenPosition), float.PositiveInfinity, m_LayerMask);
            if (rayHit == false) {
                note = null;
                return false;
            }

            note = rayHit.collider.GetComponent<Note>();
            return note;
        }

        public bool RaycastNote3D(Vector3 screenPosition, out Note note)
        {
            Ray ray = RaycastCamera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, float.PositiveInfinity, m_LayerMask)) {
                note = null;
                return false;
            }

            note = hit.collider.GetComponent<Note>();
            return note;
        }
    }
}