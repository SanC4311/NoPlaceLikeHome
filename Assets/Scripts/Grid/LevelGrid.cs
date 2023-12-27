using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    [SerializeField] private Transform gridDebugObjectPrefab;
    private GridSystem gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one LevelGrid in the scene !" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        gridSystem = new GridSystem(10, 10, 2f);
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    public void AddRiflerAtGridPosition(GridPosition gridPosition, Rifler rifler)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddRifler(rifler);
    }

    public List<Rifler> GetRiflerListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetRiflerList();
    }

    public void RemoveRiflerAtGridPosition(GridPosition gridPosition, Rifler rifler)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveRifler(rifler);
    }

    public void RiflerMovedGridPosition(Rifler rifler, GridPosition oldGridPosition, GridPosition newGridPosition)
    {
        RemoveRiflerAtGridPosition(oldGridPosition, rifler);
        AddRiflerAtGridPosition(newGridPosition, rifler);
    }
    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
}
