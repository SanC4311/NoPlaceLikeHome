using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{

    [SerializeField] private Transform gridDebugObjectPrefab;
    private GridSystem gridSystem;

    private void Awake()
    {
        gridSystem = new GridSystem(10, 10, 2f);
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    public void SetRiflerAtGridPosition(GridPosition gridPosition, Rifler rifler)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.SetRifler(rifler);
    }

    public void GetRiflerAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetRifler();
    }

    public void ClearRiflerAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.SetRifler(null);
    }
}
