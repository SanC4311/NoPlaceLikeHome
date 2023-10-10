using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiflerSelectedVisual : MonoBehaviour
{
    [SerializeField] private Rifler rifler;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        RiflerActionSystem.Instance.OnSelectedRiflerChanged += RiflerActionSystem_OnSelectedRiflerChanged;

        UpdateVisual();
    }

    private void RiflerActionSystem_OnSelectedRiflerChanged(object sender, EventArgs empty)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (RiflerActionSystem.Instance.GetSelectedRifler() == rifler)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }
}
