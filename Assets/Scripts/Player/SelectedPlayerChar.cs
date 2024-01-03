using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedPlayerChar : MonoBehaviour
{
    [SerializeField] private PlayerChar playerChar;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        PlayerActions.Instance.OnSelectedPlayerCharChanged += PlayerActions_OnSelectedPlayerCharChanged;

        UpdateVisual();
    }

    private void PlayerActions_OnSelectedPlayerCharChanged(object sender, EventArgs empty)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (PlayerActions.Instance.GetSelectedPlayerChar() == playerChar)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }
}
