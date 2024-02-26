using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRotationButtons : MonoBehaviour
{
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    private void Update()
    {
        // press Q to rotate camera left
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameInput.Instance.GetCamRotation(-1);
        }
    }
}
