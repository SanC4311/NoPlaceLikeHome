using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class GamepadCursor : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private RectTransform cursorRectTransform;
    [SerializeField] private float cursorSpeed = 1000f;
    [SerializeField] Canvas canvas;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] float padding = 20f;
    private Mouse currentMouse;
    private bool previousMouseState;
    private Mouse virtualMouse;
    private Camera mainCamera;
    private string previousControlScheme = "Keyboard&Mouse";
    private const string gamepadScheme = "Gamepad";
    private const string keyboardMouseScheme = "Keyboard&Mouse";
    public bool IsMouseControlScheme { get; private set; }

    public static GamepadCursor Instance { get; private set; }
    private void Awake()
    {
        IsMouseControlScheme = true;
        if (Instance != null)
        {
            Debug.LogError("There is more than one GamepadCursor in the scene !" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        currentMouse = Mouse.current;
        mainCamera = Camera.main;

        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorRectTransform != null)
        {
            Vector2 position = cursorRectTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        InputSystem.onAfterUpdate += UpdateMotion;
        playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnDisable()
    {
        if (virtualMouse != null && virtualMouse.added) InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null) { return; }

        Vector2 deltavalue = Gamepad.current.leftStick.ReadValue();
        deltavalue *= Time.deltaTime * cursorSpeed;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltavalue;

        newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
        newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltavalue);

        bool southButtonPressed = Gamepad.current.buttonSouth.IsPressed();

        if (previousMouseState != southButtonPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, southButtonPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = southButtonPressed;
        }

        AnchorCursor(newPosition);
    }

    private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPosition);
        cursorRectTransform.anchoredPosition = anchoredPosition;
    }

    public void OnControlsChanged(PlayerInput playerInput)
    {
        if (playerInput == null)
        {
            Debug.Log("OnControlsChanged called with null PlayerInput");
            throw new System.ArgumentNullException(nameof(playerInput));
        }

        if (playerInput.currentControlScheme == null)
        {
            Debug.Log("PlayerInput's currentControlScheme is null");
            return;
        }

        if (cursorRectTransform == null || virtualMouse == null || currentMouse == null)
        {
            Debug.Log($"Null object detected - cursorRectTransform: {cursorRectTransform}, virtualMouse: {virtualMouse}, currentMouse: {currentMouse}");
            return;
        }

        if (playerInput.currentControlScheme == keyboardMouseScheme && previousControlScheme != keyboardMouseScheme)
        {
            cursorRectTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());

            previousControlScheme = keyboardMouseScheme;

            IsMouseControlScheme = true;
        }
        else if (playerInput.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
        {
            cursorRectTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
            AnchorCursor(currentMouse.position.ReadValue());

            previousControlScheme = gamepadScheme;

            IsMouseControlScheme = false;
        }
    }



}
