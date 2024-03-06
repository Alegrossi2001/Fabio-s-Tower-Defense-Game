using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool isGameOnMobile;

    //zoom
    private float zoomSpeed = 3f;
    private float minZoom = 10f;
    private float maxZoom = 100f;
    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOnMobile)
        {
            if (Touchscreen.current.primaryTouch.press.isPressed)
            {
                Vector2 touchDelta = Touchscreen.current.primaryTouch.delta.ReadValue() * Time.deltaTime;
                Vector3 move = new Vector3(touchDelta.x, 0, touchDelta.y);
                transform.Translate(move * moveSpeed);
            }
            //zoom using touch pinch gesture
            if (Touchscreen.current.primaryTouch.isInProgress)
            {
                var touch = Touchscreen.current.primaryTouch;
                float pinchDelta = touch.delta.ReadValue().magnitude;
                ZoomCamera(-pinchDelta * zoomSpeed);
            }
            
        }
        else
        {
            // Read movement input using the new Input System
            Vector2 movementInput = Keyboard.current.wKey.isPressed ? Vector2.up :
                            Keyboard.current.sKey.isPressed ? Vector2.down :
                            Keyboard.current.aKey.isPressed ? Vector2.left :
                            Keyboard.current.dKey.isPressed ? Vector2.right :
                            Vector2.zero;

            Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
            transform.Translate(move * moveSpeed * Time.deltaTime);

            //zoom using mouse scroll wheel
            float scrollDelta = Mouse.current.scroll.ReadValue().y;
            ZoomCamera(-scrollDelta * zoomSpeed);
        }

    }

    void ZoomCamera(float zoomAmount)
    {
        float targetFOV = Mathf.Clamp(camera.fieldOfView + zoomAmount, minZoom, maxZoom);
        float newFOV = Mathf.Lerp(camera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        camera.fieldOfView = newFOV;
    }
}
