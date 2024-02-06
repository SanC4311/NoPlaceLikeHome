using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private GameObject moveButton;
    [SerializeField] private GameObject assaultRifleButton;
    [SerializeField] private GameObject shotgunButton;
    [SerializeField] private GameObject repairButton;
    [SerializeField] private GameObject ammoButton;
    [SerializeField] private Button button;

    public void SetPlayerControl(PlayerControl playerControl)
    {
        // Clear buttons by disabling them
        moveButton.gameObject.SetActive(false);
        assaultRifleButton.gameObject.SetActive(false);
        shotgunButton.gameObject.SetActive(false);
        repairButton.gameObject.SetActive(false);
        ammoButton.gameObject.SetActive(false);

        PlayerControl currentControl = playerControl;
        Debug.Log(currentControl);

        if (currentControl is PlayerMove)
        {
            moveButton.gameObject.SetActive(true);
        }
        else if (playerControl is PlayerRepair)
        {
            repairButton.gameObject.SetActive(true);
        }
        else if (playerControl is PlayerRifle)
        {
            assaultRifleButton.gameObject.SetActive(true);
        }
        else if (playerControl is PlayerShotgun)
        {
            shotgunButton.gameObject.SetActive(true);
        }
        else if (playerControl is PlayerAmmo)
        {
            ammoButton.gameObject.SetActive(true);
        }

        button.onClick.AddListener(() =>
        {
            PlayerActions.Instance.SetSelectedControl(currentControl);
        });
    }
}
