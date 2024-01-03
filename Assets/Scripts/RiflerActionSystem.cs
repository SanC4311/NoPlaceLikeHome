using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharActionSystem : MonoBehaviour
{

    public static PlayerCharActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedPlayerCharChanged;

    [SerializeField] private PlayerChar selectedPlayerChar;
    [SerializeField] private LayerMask playerCharLayerMask;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one PlayerCharActionSystem in the scene !" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {

        if (GameInput.Instance.isLeftMouseButtonDownThisFrame())
        {
            if (TryHandlePlayerCharSelection()) return;
            selectedPlayerChar.Move(MouseWorld.GetPosition());
        }

    }

    private bool TryHandlePlayerCharSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(GameInput.Instance.GetMouseScreenPosition());
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, playerCharLayerMask))
        {
            if (raycastHit.transform.TryGetComponent<PlayerChar>(out PlayerChar playerChar))
            {
                SetSelectedPlayerChar(playerChar);
                return true;
            }
        }
        return false;

    }

    private void SetSelectedPlayerChar(PlayerChar playerChar)
    {
        selectedPlayerChar = playerChar;
        OnSelectedPlayerCharChanged?.Invoke(this, EventArgs.Empty);
    }

    public PlayerChar GetSelectedPlayerChar()
    {
        return selectedPlayerChar;
    }

}
