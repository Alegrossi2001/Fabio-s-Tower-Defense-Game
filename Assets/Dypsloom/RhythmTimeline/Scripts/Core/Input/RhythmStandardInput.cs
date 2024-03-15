/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Core.Input
{
    using System;
    using Dypsloom.RhythmTimeline.Core.Managers;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Input event data tracks the input type.
    /// </summary>
    public class InputEventData
    {
        public int InputID;
        public int TrackID;
        public Vector2 Direction;

        public virtual bool Tap => InputID == 0;
        public virtual bool Release => InputID == 1;
        public virtual bool Swipe => InputID == 2;

        public InputEventData(int trackID, int inputID)
        {
            TrackID = trackID;
            InputID = inputID;
        }

    }

    /// <summary>
    /// A simple abstraction to allow either key or button input.
    /// </summary>
    [Serializable]
    public struct SimpleTrackInput
    {
        [SerializeField] private KeyCode m_Key;
        [SerializeField] private string m_Button;

        public SimpleTrackInput(KeyCode key, string button = null)
        {
            m_Key = key;
            m_Button = button;
        }
        
        public bool GetInputDown()
        {
            var input = false;
            
            if (m_Key != KeyCode.None) {
                input |= Input.GetKeyDown(m_Key);
            }

            if (string.IsNullOrWhiteSpace(m_Button) == false) {
                input |= Input.GetButtonDown(m_Button);
            }

            return input;
        }
        
        public bool GetInputUp()
        {
            var input = false;
            
            if (m_Key != KeyCode.None) {
                input |= Input.GetKeyUp(m_Key);
            }

            if (string.IsNullOrWhiteSpace(m_Button) == false) {
                input |= Input.GetButtonUp(m_Button);
            }

            return input;
        }
        
        public bool GetInput()
        {
            var input = false;
            
            if (m_Key != KeyCode.None) {
                input |= Input.GetKey(m_Key);
            }

            if (string.IsNullOrWhiteSpace(m_Button) == false) {
                input |= Input.GetButton(m_Button);
            }

            return input;
        }
    }

    /// <summary>
    /// Gets information from the RhythmDirector and from the input to processes notes.
    /// </summary>
    public class RhythmStandardInput : MonoBehaviour
    {
        public const int c_MouseTouchFingerID = -1;
        
        [Tooltip("The Rhythm Processor.")]
        [SerializeField] protected RhythmProcessor m_RhythmProcessor;
        [Tooltip("The Key input for each track.")]
        [SerializeField] protected SimpleTrackInput[] m_TrackInput;
        [Tooltip("The key code to swipe left.")]
        [SerializeField] protected SimpleTrackInput m_SwipeLeft = new SimpleTrackInput(KeyCode.LeftArrow);
        [Tooltip("The key code to swipe right.")]
        [SerializeField] protected SimpleTrackInput m_SwipeRight = new SimpleTrackInput(KeyCode.RightArrow);
        [Tooltip("The key code to swipe up.")]
        [SerializeField] protected SimpleTrackInput m_SwipeUp = new SimpleTrackInput(KeyCode.UpArrow);
        [Tooltip("The key code to swipe down.")]
        [SerializeField] protected SimpleTrackInput m_SwipeDown = new SimpleTrackInput(KeyCode.DownArrow);
        [Tooltip("Use the 2D or 3D Collider of the Track Object for touch detection?")]
        [SerializeField] protected bool m_2DTouchCollider = false;
        [Tooltip("The layer mask for the touch input collider.")]
        [SerializeField] protected LayerMask m_TouchInputMask;
        [Tooltip("The threshold movement for detecting a swipe.")]
        [SerializeField] protected float m_SwipeThreshold = 20;
        [Tooltip("Check for swipes only when releasing the input?")]
        [SerializeField] protected bool m_DetectSwipeOnlyAfterRelease = false;
        [Tooltip("Disable Key input on Editor")]
        [SerializeField] protected bool m_DisableKeyInputInEditor = false;
        [Tooltip("Disable Touch input on Editor")]
        [SerializeField] protected bool m_DisableTouchInputInEditor = false;
        [Tooltip("Disable the Mouse input on Editor")]
        [SerializeField] protected bool m_DisableMouseInputInEditor = false;
        [Tooltip("Disable Key input on build")]
        [SerializeField] protected bool m_DisableKeyInputInBuild = false;
        [Tooltip("Disable Touch input on build")]
        [SerializeField] protected bool m_DisableTouchInputInBuild = false;
        [Tooltip("Disable the Mouse input on build")]
        [SerializeField] protected bool m_DisableMouseInputInBuild = false;

        protected InputEventData[] m_TrackInputEventData;
        protected Camera m_Camera;
        protected Dictionary<int, Vector2> m_TouchBeganPosition;
        protected Dictionary<int, Vector2> m_TouchEndedPosition;
        protected Dictionary<int, int> m_TouchToTrackMap;

        protected virtual void Awake()
        {
            m_Camera = Camera.main;

            m_TrackInputEventData = new InputEventData[m_TrackInput.Length];
            for (int i = 0; i < m_TrackInputEventData.Length; i++) {
                m_TrackInputEventData[i] = new InputEventData(i, -1);
            }

            m_TouchBeganPosition = new Dictionary<int, Vector2>();
            m_TouchEndedPosition = new Dictionary<int, Vector2>();
            m_TouchToTrackMap = new Dictionary<int, int>();
        }

        public virtual void Update()
        {
#if UNITY_EDITOR
            if (!m_DisableKeyInputInEditor) {
                TickKeyInput();
            }

            if (!m_DisableTouchInputInEditor) {
                TickTouch();
            }

            if (!m_DisableMouseInputInEditor) {
                TickMouseClick();
            }
            
#else
            if (!m_DisableKeyInputInBuild) {
                TickKeyInput();
            }

            if (!m_DisableTouchInputInBuild) {
                TickTouch();
            }

            if (!m_DisableMouseInputInBuild) {
                TickMouseClick();
            }
#endif

        }

        public virtual void TickKeyInput()
        {
            for (int i = 0; i < m_TrackInput.Length; i++) {
                var input = m_TrackInput[i];
                var trackInputEventData = m_TrackInputEventData[i];

                if (input.GetInputDown()) { TriggerInput(trackInputEventData, 0); }

                if (input.GetInputUp()) { TriggerInput(trackInputEventData, 1); }

                // Trigger swipe input if the swipe input is pressed while the track button is hold.
                if (input.GetInput()) {
                    if (m_SwipeDown.GetInputDown()) {
                        TriggerInput(trackInputEventData, 2, Vector2.down);
                    } 
                    if (m_SwipeUp.GetInputDown()) {
                        TriggerInput(trackInputEventData, 2, Vector2.up);
                    } 
                    if (m_SwipeLeft.GetInputDown()) {
                        TriggerInput(trackInputEventData, 2, Vector2.left);
                    } 
                    if (m_SwipeRight.GetInputDown()) {
                        TriggerInput(trackInputEventData, 2, Vector2.right);
                    }
                }
            }
        }

        public virtual void TickTouch()
        {
            for (int i = 0; i < Input.touches.Length; i++) {
                var touch = Input.touches[i];
                var inputPosition = touch.position;

                //previous touche can be released on another track
                if (m_TouchToTrackMap.TryGetValue(touch.fingerId, out var trackID) && trackID != -1) {

                    var previousTrackInputEventData = m_TrackInputEventData[trackID];
                        
                    //Detects Swipe while finger is still moving
                    if (touch.phase == TouchPhase.Moved) {
                        InputMoved(touch.fingerId, previousTrackInputEventData, touch.position);
                    }

                    if (touch.phase == TouchPhase.Ended) {
                        InputReleased(touch.fingerId, previousTrackInputEventData, touch.position);
                    }
                } else {
                    trackID = -1;
                }

                var trackInputEventData = GetTrackInputEventData(inputPosition);

                if (trackInputEventData == null) { continue; }
                
                //The input event was already checked for this track
                if(trackID == trackInputEventData.TrackID){ continue; }

                if (touch.phase == TouchPhase.Began) {
                    InputPressed(touch.fingerId, trackInputEventData, touch.position);
                }

                //Detects Swipe while finger is still moving
                if (touch.phase == TouchPhase.Moved) {
                    InputMoved(touch.fingerId, trackInputEventData, touch.position);
                }

                if (touch.phase == TouchPhase.Ended) {
                    InputReleased(touch.fingerId, trackInputEventData, touch.position);
                }
            }
        }

        void CheckSwipe(InputEventData trackInputEventData, int fingerId)
        {
            if (m_TouchEndedPosition.ContainsKey(fingerId) == false ||
                m_TouchBeganPosition.ContainsKey(fingerId) == false) {
                return;
            }
            var direction = m_TouchEndedPosition[fingerId] - m_TouchBeganPosition[fingerId];
            var sqrDistance = direction.sqrMagnitude;
            
            if (sqrDistance > m_SwipeThreshold) { TriggerInput(trackInputEventData, 2, direction); }
        }

        private void TickMouseClick()
        {

            if (Input.GetMouseButtonDown(0)) {

                var inputPosition = Input.mousePosition;

                var trackInputEventData = GetTrackInputEventData(inputPosition);

                if (trackInputEventData == null) { return; }

                InputPressed(c_MouseTouchFingerID, trackInputEventData, inputPosition);
            }
            
            if (!m_DetectSwipeOnlyAfterRelease && Input.GetMouseButton(0)) {

                var inputPosition = Input.mousePosition;

                //Check for previous track
                if (m_TouchToTrackMap.TryGetValue(c_MouseTouchFingerID, out var trackID) && trackID != -1) {
                    var previousTrackInputEventData = m_TrackInputEventData[trackID];
                    InputMoved(c_MouseTouchFingerID, previousTrackInputEventData, inputPosition);
                    
                } else {
                    trackID = -1;
                }
                var trackInputEventData = GetTrackInputEventData(inputPosition);

                if (trackInputEventData == null || trackInputEventData.TrackID == trackID) { return; }
                
                InputMoved(c_MouseTouchFingerID, trackInputEventData, inputPosition);
            }

            if (Input.GetMouseButtonUp(0)) {

                var inputPosition = Input.mousePosition;

                //Chceck for previous track
                if (m_TouchToTrackMap.TryGetValue(c_MouseTouchFingerID, out var trackID) && trackID != -1) {
                    var previousTrackInputEventData = m_TrackInputEventData[trackID];
                    InputReleased(c_MouseTouchFingerID, previousTrackInputEventData, inputPosition);
                } else {
                    trackID = -1;
                }
                var trackInputEventData = GetTrackInputEventData(inputPosition);

                if (trackInputEventData == null || trackInputEventData.TrackID == trackID) { return; }

                InputReleased(c_MouseTouchFingerID, trackInputEventData, inputPosition);
            }
        }

        protected virtual void InputPressed(int fingerID, InputEventData trackInputEventData, Vector3 inputPosition)
        {
            TriggerInput(trackInputEventData, 0);
            m_TouchBeganPosition[fingerID] = inputPosition;
            m_TouchEndedPosition[fingerID] = inputPosition;
            m_TouchToTrackMap[fingerID] = trackInputEventData.TrackID;
        }

        protected virtual void InputMoved(int fingerID, InputEventData previousTrackInputEventData, Vector3 inputPosition)
        {
            if (m_DetectSwipeOnlyAfterRelease) { return; }

            m_TouchEndedPosition[fingerID] = inputPosition;
            CheckSwipe(previousTrackInputEventData, fingerID);
        }
        
        protected virtual void InputReleased(int fingerID, InputEventData previousTrackInputEventData, Vector3 inputPosition)
        {
            TriggerInput(previousTrackInputEventData, 1);
            m_TouchEndedPosition[fingerID] = inputPosition;
            m_TouchToTrackMap[fingerID] = -1;
            CheckSwipe(previousTrackInputEventData, fingerID);
        }

        protected virtual InputEventData GetTrackInputEventData(Vector2 inputPosition)
        {
            var tackObjects = m_RhythmProcessor.RhythmDirector.TrackObjects;
            
            var ray = m_Camera.ScreenPointToRay(inputPosition);
            
            if (m_2DTouchCollider) {
                
                var hit = Physics2D.Raycast(ray.origin, ray.direction, 100, m_TouchInputMask);
                
                if (hit.collider == null) { return null; }

                for (int i = 0; i < tackObjects.Length; i++) {
                    if (hit.collider != tackObjects[i].TouchCollider2D) { continue; }

                    return m_TrackInputEventData[i];

                }

                return null;
            }
            
            if (Physics.Raycast(ray, out var hitInfo, 100, m_TouchInputMask) == false) { return null; }

            for (int i = 0; i < tackObjects.Length; i++) {
                if (hitInfo.collider != tackObjects[i].TouchCollider3D) { continue; }

                return m_TrackInputEventData[i];
            }

            return null;
        }

        protected virtual void TriggerInput(InputEventData trackInputEventData, int inputID, Vector2 direction)
        {
            trackInputEventData.InputID = inputID;
            trackInputEventData.Direction = direction;
            TriggerInput(trackInputEventData);
        }

        protected virtual void TriggerInput(InputEventData trackInputEventData, int inputID)
        {
            trackInputEventData.InputID = inputID;
            trackInputEventData.Direction = Vector2.zero;
            TriggerInput(trackInputEventData);
        }

        protected virtual void TriggerInput(InputEventData trackInputEventData)
        {
            m_RhythmProcessor.TriggerInput(trackInputEventData);
        }
    }
}