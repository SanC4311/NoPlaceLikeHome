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
        Vector2 inputMoveDirection = GameInput.Instance.GetCamMoveVector();

        float camSpeed = 10f;

        Vector3 movement = transform.forward * inputMoveDirection.y + transform.right * inputMoveDirection.x;
        transform.position += movement * camSpeed * Time.deltaTime;
    }

    private void camRotation()
    {
        Vector3 rotVector = new Vector3(0, 0, 0);
        rotVector.y = GameInput.Instance.GetCamRotation();

        float rotSpeed = 100f;

        transform.eulerAngles += rotVector * rotSpeed * Time.deltaTime;
    }

    private void camZoom()
    {
        float zoomIncAmount = 1f;
        offset.y += GameInput.Instance.GetCamZoom() * zoomIncAmount;

        offset.y = Mathf.Clamp(offset.y, Y_ANGLE_MIN, Y_ANGLE_MAX);

        float zoomSmooth = 5f;
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, offset, Time.deltaTime * zoomSmooth);
    }
}
