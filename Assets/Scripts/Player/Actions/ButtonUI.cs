using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private GameObject moveButton;
    [SerializeField] private GameObject assaultRifleButton;
    [SerializeField] private GameObject shotgunButton;
    [SerializeField] private GameObject repairButton;
    [SerializeField] private GameObject ammoButton;

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
        // else if (playerControl is PlayerAssaultRifle)
        // {
        //     Instantiate(assaultRifleButton, button);
        // }
        // else if (playerControl is PlayerShotgun)
        // {
        //     Instantiate(shotgunButton, button);
        // }
        else if (playerControl is PlayerTest)
        {
            repairButton.gameObject.SetActive(true);
        }
        // else if (playerControl is PlayerAmmo)
        // {
        //     Instantiate(ammoButton, button);
        // }
    }
}
