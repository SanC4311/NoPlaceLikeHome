using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharSelectedVisual : MonoBehaviour
{
    [SerializeField] private PlayerChar playerChar;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        PlayerCharActionSystem.Instance.OnSelectedPlayerCharChanged += PlayerCharActionSystem_OnSelectedPlayerCharChanged;

        UpdateVisual();
    }

    private void PlayerCharActionSystem_OnSelectedPlayerCharChanged(object sender, EventArgs empty)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (PlayerCharActionSystem.Instance.GetSelectedPlayerChar() == playerChar)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }
}
