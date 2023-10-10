using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiflerActionSystem : MonoBehaviour
{

    public static RiflerActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedRiflerChanged;

    [SerializeField] private Rifler selectedRifler;
    [SerializeField] private LayerMask riflerLayerMask;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one RiflerActionSystem in the scene !" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (TryHandleRiflerSelection()) return;
            selectedRifler.Move(MouseWorld.GetPosition());
        }

    }

    private bool TryHandleRiflerSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, riflerLayerMask))
        {
            if (raycastHit.transform.TryGetComponent<Rifler>(out Rifler rifler))
            {
                SetSelectedRifler(rifler);
                return true;
            }
        }
        return false;

    }

    private void SetSelectedRifler(Rifler rifler)
    {
        selectedRifler = rifler;
        OnSelectedRiflerChanged?.Invoke(this, EventArgs.Empty);
    }

    public Rifler GetSelectedRifler()
    {
        return selectedRifler;
    }

}
