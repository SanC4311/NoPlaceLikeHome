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

        gridSystem = new GridSystem(5, 5, 2f);
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
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
    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);
    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();

    public bool HasAnyPlayerCharAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyPlayerChar();
    }

    public DefenseHealthUI GetDefenseAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetDefense();
    }

    public void SetDefenseAtGridPosition(GridPosition gridPosition, DefenseHealthUI defense)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.SetDefense(defense);
    }

}
