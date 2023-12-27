using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridSystem gridSystem;
    private GridPosition gridPosition;

    private List<Rifler> riflerList;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        riflerList = new List<Rifler>();
    }

    public override string ToString()
    {
        string riflerString = "";
        foreach (Rifler rifler in riflerList)
        {
            riflerString += rifler + "\n";
        }
        return gridPosition.ToString() + "\n" + riflerString;
    }

    public void AddRifler(Rifler rifler)
    {
        riflerList.Add(rifler);
    }

    public void RemoveRifler(Rifler rifler)
    {
        riflerList.Remove(rifler);
    }

    public List<Rifler> GetRiflerList()
    {
        return riflerList;
    }

}
