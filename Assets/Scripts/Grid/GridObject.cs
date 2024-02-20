using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridSystem gridSystem;
    private GridPosition gridPosition;

    private List<PlayerChar> playerCharList;

    private DefenseHealthUI defense;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        playerCharList = new List<PlayerChar>();
    }

    public override string ToString()
    {
        string playerCharString = "";
        foreach (PlayerChar playerChar in playerCharList)
        {
            playerCharString += playerChar + "\n";
        }
        return gridPosition.ToString() + "\n" + playerCharString;
    }

    public void AddPlayerChar(PlayerChar playerChar)
    {
        playerCharList.Add(playerChar);
    }

    public void RemovePlayerChar(PlayerChar playerChar)
    {
        playerCharList.Remove(playerChar);
    }

    public List<PlayerChar> GetPlayerCharList()
    {
        return playerCharList;
    }

    public bool HasAnyPlayerChar()
    {
        return playerCharList.Count > 0;
    }

    public DefenseHealthUI GetDefense()
    {
        return defense;
    }
    public void SetDefense(DefenseHealthUI defense)
    {
        this.defense = defense;
    }


}
