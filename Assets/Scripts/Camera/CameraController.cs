using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    // Update is called once per frame
    void Update()
    {
        camRotation();
    }

    private void camRotation()
    {
        Vector3 rotVector = new Vector3(0, 0, 0);
        rotVector.y = GameInput.Instance.GetCamRotation();

        float rotSpeed = 100f;

        transform.eulerAngles += rotVector * rotSpeed * Time.deltaTime;
    }
}
