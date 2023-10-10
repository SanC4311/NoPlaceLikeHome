using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{

    private static MouseWorld instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Update()
    {
        transform.position = MouseWorld.GetPosition();
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        return raycastHit.point;
    }

}
