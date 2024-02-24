using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    //If left button is pressed, rotate camera left
    public void StartRotateLeft()
    {

        // Keep rotating 
        InvokeRepeating("RotateLeft", 0, 0.01f);
        Vector3 rotVector = new Vector3(0, 0, 0);
        rotVector.y = -0.5f;

        float rotSpeed = 100f;

        transform.eulerAngles += rotVector * rotSpeed * Time.deltaTime;
    }

    public void RotateLeft()
    {
        Vector3 rotVector = new Vector3(0, 0, 0);
        rotVector.y = -0.5f;

        float rotSpeed = 100f;

        transform.eulerAngles += rotVector * rotSpeed * Time.deltaTime;
    }

    //If left button is released, stop rotating camera left
    public void StopRotateLeft()
    {
        CancelInvoke("RotateLeft");
    }

    //If right button is pressed, rotate camera right
    public void StartRotateRight()
    {
        // Keep rotating 
        InvokeRepeating("RotateRight", 0, 0.01f);
        Vector3 rotVector = new Vector3(0, 0, 0);
        rotVector.y = 0.5f;

        float rotSpeed = 100f;

        transform.eulerAngles += rotVector * rotSpeed * Time.deltaTime;
    }

    public void RotateRight()
    {
        Vector3 rotVector = new Vector3(0, 0, 0);
        rotVector.y = 0.5f;

        float rotSpeed = 100f;

        transform.eulerAngles += rotVector * rotSpeed * Time.deltaTime;
    }

    //If right button is released, stop rotating camera right
    public void StopRotateRight()
    {
        CancelInvoke("RotateRight");
    }
}
