using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] private Transform controlButton;
    [SerializeField] private Transform buttonContainer;

    // Start is called before the first frame update
    void Start()
    {
        PlayerActions.Instance.OnSelectedPlayerCharChanged += PlayerActions_OnSelectedPlayerCharChanged;
        CreatePlayerUI();
    }


    private void CreatePlayerUI()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        PlayerChar selectedPlayerChar = PlayerActions.Instance.GetSelectedPlayerChar();

        foreach (PlayerControl playerControl in selectedPlayerChar.GetPlayerControls())
        {
            Transform buttonTransform = Instantiate(controlButton, buttonContainer);
            ButtonUI buttonUI = buttonTransform.GetComponent<ButtonUI>();
            buttonUI.SetPlayerControl(playerControl);
        }
    }

    private void PlayerActions_OnSelectedPlayerCharChanged(object sender, EventArgs e)
    {
        CreatePlayerUI();
    }
}
