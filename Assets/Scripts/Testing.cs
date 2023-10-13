using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    private GridSystem gridSystem;
    [SerializeField] private Transform gridDebugObjectPrefab;
    // Start is called before the first frame update
    private void Start()
    {
        gridSystem = new GridSystem(10, 10, 2f);
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        Debug.Log(gridSystem.GetWorldPosition(5, 7));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(gridSystem.GetGridPosition(MouseWorld.GetPosition()));
        }
    }
}
