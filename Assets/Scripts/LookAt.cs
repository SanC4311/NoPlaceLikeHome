using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private enum Mode
    {
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted
    }

    [SerializeField] private Vector3 rotateOffset;
    [SerializeField] private Mode mode;

    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                transform.LookAt(Camera.main.transform);
                transform.Rotate(rotateOffset);
                break;
            case Mode.CameraForward:
                transform.rotation = Camera.main.transform.rotation;
                break;
            case Mode.CameraForwardInverted:
                transform.rotation = Camera.main.transform.rotation;
                transform.Rotate(rotateOffset);
                break;
        }
    }
}
