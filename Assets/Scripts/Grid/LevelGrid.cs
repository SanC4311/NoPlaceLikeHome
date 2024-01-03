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

    public void AddPlayerCharAtGridPosition(GridPosition gridPosition, PlayerChar playerChar)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddPlayerChar(playerChar);
    }

    public List<PlayerChar> GetPlayerCharListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetPlayerCharList();
    }

    public void RemovePlayerCharAtGridPosition(GridPosition gridPosition, PlayerChar playerChar)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemovePlayerChar(playerChar);
    }

    public void PlayerCharMovedGridPosition(PlayerChar playerChar, GridPosition oldGridPosition, GridPosition newGridPosition)
    {
        RemovePlayerCharAtGridPosition(oldGridPosition, playerChar);
        AddPlayerCharAtGridPosition(newGridPosition, playerChar);
    }
    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
}
