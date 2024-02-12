using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefendShotgun : PlayerControl
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
