#define USE_NEW_INPUT_SYSTEM
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;  // New Input System

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private GameInputActions gameInputActions;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GameInput in the scene !" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        gameInputActions = new GameInputActions();
        gameInputActions.Player.Enable();
    }

    public Vector2 GetMouseScreenPosition()
    {
        // Debug.Log("InputMouse: " + Input.mousePosition);
        // Debug.Log("InputVirtualMouse: " + gameInputActions.Player.VirtualMousePosition.ReadValue<Vector2>());

#if USE_NEW_INPUT_SYSTEM
        if (GamepadCursor.Instance.IsMouseControlScheme)
        {
            return Input.mousePosition;
        }
        else
        {
            return gameInputActions.Player.VirtualMousePosition.ReadValue<Vector2>();
        }
#else
        return Input.mousePosition;
#endif
    }

    public bool isLeftMouseButtonDownThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return gameInputActions.Player.LeftMouseClick.WasPressedThisFrame();
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    public float GetCamRotation()
    {
#if USE_NEW_INPUT_SYSTEM
        return gameInputActions.Player.CamRotate.ReadValue<float>();
#else
        float rotation = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotation -= 1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotation += 1f;
        }

        return rotation;
#endif
    }
}
