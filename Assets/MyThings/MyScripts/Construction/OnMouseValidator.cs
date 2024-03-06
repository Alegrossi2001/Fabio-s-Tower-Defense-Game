using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OnMouseValidator : MonoBehaviour
{
    //touch countrols
    private Camera camera;
    [SerializeField] private bool isGameOnMobile;
    [SerializeField] private LayerMask groundLayer;
    //event emitter
    public static EventHandler<OnTouchEventArgs> onTouch;

    private void Awake()
    {
        camera = Camera.main;        
    }
    void Update()
    {
        if(BuildingOptions.buildingOptionsActive == true)
        {
            if (isGameOnMobile)
            {
                if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                {
                    Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                    Ray ray = camera.ScreenPointToRay(touchPosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (CheckForObstruction(hit.collider))
                        {
                            onTouch?.Invoke(this, new OnTouchEventArgs
                            {
                                collider = hit.collider,
                            });
                        }
                    }
                }
            }
            else
            {
                Mouse mouse = Mouse.current;
                if (mouse.leftButton.wasPressedThisFrame)
                {
                    Vector3 mousePosition = mouse.position.ReadValue();
                    Ray ray = camera.ScreenPointToRay(mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (CheckForObstruction(hit.collider))
                        {
                            onTouch?.Invoke(this, new OnTouchEventArgs
                            {
                                collider = hit.collider,
                            });
                        }
                    }
                }

            }

        }
    }

    private bool CheckForObstruction(Collider collider)
    {
        if(collider.gameObject.layer == 6)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
