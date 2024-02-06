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
    [SerializeField] private GameObject selectedVisual;
    [SerializeField] private Button button;

    private PlayerControl currentControl;

    public void SetPlayerControl(PlayerControl playerControl)
    {
        // Clear buttons by disabling them
        moveButton.gameObject.SetActive(false);
        assaultRifleButton.gameObject.SetActive(false);
        shotgunButton.gameObject.SetActive(false);
        repairButton.gameObject.SetActive(false);
        ammoButton.gameObject.SetActive(false);

        this.currentControl = playerControl;
        Debug.Log(currentControl);

        if (this.currentControl is PlayerMove)
        {
            moveButton.gameObject.SetActive(true);
        }
        else if (this.currentControl is PlayerRepair)
        {
            repairButton.gameObject.SetActive(true);
        }
        else if (this.currentControl is PlayerRifle)
        {
            assaultRifleButton.gameObject.SetActive(true);
        }
        else if (this.currentControl is PlayerShotgun)
        {
            shotgunButton.gameObject.SetActive(true);
        }
        else if (this.currentControl is PlayerAmmo)
        {
            ammoButton.gameObject.SetActive(true);
        }

        button.onClick.AddListener(() =>
        {
            PlayerActions.Instance.SetSelectedControl(currentControl);
        });
    }

    public void UpdateVisual()
    {
        PlayerControl selectedControl = PlayerActions.Instance.GetSelectedControl();
        selectedVisual.SetActive(selectedControl == currentControl);
    }
}
