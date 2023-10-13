using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private gridSystem gridSystem;
    private gridPosition gridPosition;

    public GridObject(gridSystem gridSystem, gridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
    }

}
