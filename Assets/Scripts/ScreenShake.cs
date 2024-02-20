using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{

    public static ScreenShake Instance { get; private set; }
    private CinemachineImpulseSource cinemachineImpulseSource;

    private void Awake()
    {

        if (Instance != null)
        {
            Debug.LogError("There's more than one ScreenShake instance in the scene! " + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void ShakeCamera(float shakeIntensity = 1f)
    {
        cinemachineImpulseSource.GenerateImpulse(shakeIntensity);
    }
}
