using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    private const float Y_ANGLE_MIN = 1.6f;
    private const float Y_ANGLE_MAX = 7f;

    private Vector3 offset;

    private CinemachineTransposer cinemachineTransposer;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        cinemachineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        offset = cinemachineTransposer.m_FollowOffset;

    }
    // Update is called once per frame
    void Update()
    {
        camMovement();
        camRotation();
        camZoom();
    }

    private void camMovement()
    {
        Vector3 inputMoveDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDirection.z += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDirection.z -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDirection.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDirection.x += 1;
        }

        float camSpeed = 10f;
        Vector3 movement = transform.forward * inputMoveDirection.z + transform.right * inputMoveDirection.x;
        transform.position += movement * camSpeed * Time.deltaTime;
    }

    private void camRotation()
    {
        Vector3 rotVector = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.Q))
        {
            rotVector.y -= 1;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotVector.y += 1;
        }

        float rotSpeed = 100f;

        transform.eulerAngles += rotVector * rotSpeed * Time.deltaTime;
    }

    private void camZoom()
    {
        float zoomAmount = 1f;
        if (Input.mouseScrollDelta.y > 0)
        {
            offset.y -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            offset.y += zoomAmount;
        }
        offset.y = Mathf.Clamp(offset.y, Y_ANGLE_MIN, Y_ANGLE_MAX);

        float zoomSmooth = 5f;
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, offset, Time.deltaTime * zoomSmooth);
    }
}
