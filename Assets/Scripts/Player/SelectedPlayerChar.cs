using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedPlayerChar : MonoBehaviour
{
    [SerializeField] private PlayerChar playerChar;

    public GameObject playerCharGameObject;
    private Material material;

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
            playerCharGameObject.SetActive(true);

        }
        else
        {
            playerCharGameObject.SetActive(false);
        }
    }
}
