using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    private GridSystem gridSystem;
    // Start is called before the first frame update
    private void Start()
    {
        gridSystem = new GridSystem(10, 10, 2f);

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
