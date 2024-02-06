using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] private Transform controlButton;
    [SerializeField] private Transform buttonContainer;

    private List<ButtonUI> buttonUIList;

    private void Awake()
    {
        buttonUIList = new List<ButtonUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerActions.Instance.OnSelectedPlayerCharChanged += PlayerActions_OnSelectedPlayerCharChanged;
        PlayerActions.Instance.OnSelectedControlChanged += PlayerActions_OnSelectedControlChanged;

        CreatePlayerUI();
        UpdateVisual();
    }


    private void CreatePlayerUI()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        buttonUIList.Clear();

        PlayerChar selectedPlayerChar = PlayerActions.Instance.GetSelectedPlayerChar();

        foreach (PlayerControl playerControl in selectedPlayerChar.GetPlayerControls())
        {
            Transform buttonTransform = Instantiate(controlButton, buttonContainer);
            ButtonUI buttonUI = buttonTransform.GetComponent<ButtonUI>();
            buttonUI.SetPlayerControl(playerControl);

            buttonUIList.Add(buttonUI);
        }
    }

    private void PlayerActions_OnSelectedPlayerCharChanged(object sender, EventArgs e)
    {
        CreatePlayerUI();
        UpdateVisual();
    }

    private void PlayerActions_OnSelectedControlChanged(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (ButtonUI buttonUI in buttonUIList)
        {
            buttonUI.UpdateVisual();
        }
    }
}
