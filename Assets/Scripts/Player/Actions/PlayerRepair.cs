using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerRepair : PlayerControl
{
    public override void DoControl(GridPosition gridPosition, System.Action onActionComplete)
    {
        throw new System.NotImplementedException();
    }

    public override List<GridPosition> GetValidPositionList()
    {
        GridPosition playerCharGridPosition = playerChar.GetGridPosition();

        return new List<GridPosition>
        {
            playerCharGridPosition
        };
    }
}
