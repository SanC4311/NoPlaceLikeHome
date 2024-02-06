using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerActions : MonoBehaviour
{

    public static PlayerActions Instance { get; private set; }

    public event EventHandler OnSelectedPlayerCharChanged;

    [SerializeField] private PlayerChar selectedPlayerChar;
    [SerializeField] private LayerMask playerCharLayerMask;
    private PlayerControl selectedControl;
    private bool preoccupied;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one PlayerActions in the scene !" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetSelectedPlayerChar(selectedPlayerChar);
    }

    private void Update()
    {
        if (preoccupied) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (TryHandlePlayerCharSelection()) return;
        HandleControl();

    }

    private void HandleControl()
    {
        if (GameInput.Instance.isLeftMouseButtonDownThisFrame())
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (selectedControl.IsValidPosition(mouseGridPosition))
            {
                SetPreoccupied();
                selectedControl.DoControl(mouseGridPosition, ClearPreoccupied);
            }
        }
    }

    private void SetPreoccupied()
    {
        preoccupied = true;
    }

    private void ClearPreoccupied()
    {
        preoccupied = false;
    }

    private bool TryHandlePlayerCharSelection()
    {
        if (GameInput.Instance.isLeftMouseButtonDownThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(GameInput.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, playerCharLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<PlayerChar>(out PlayerChar playerChar))
                {
                    if (playerChar == selectedPlayerChar)
                    {
                        return false;
                    }
                    SetSelectedPlayerChar(playerChar);
                    return true;
                }
            }
            return false;
        }

        return false;
    }

    private void SetSelectedPlayerChar(PlayerChar playerChar)
    {
        selectedPlayerChar = playerChar;
        SetSelectedControl(selectedPlayerChar.GetPlayerMove());
        OnSelectedPlayerCharChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedControl(PlayerControl playerControl)
    {
        selectedControl = playerControl;
    }

    public PlayerChar GetSelectedPlayerChar()
    {
        return selectedPlayerChar;
    }

    public PlayerControl GetSelectedControl()
    {
        return selectedControl;
    }

}
