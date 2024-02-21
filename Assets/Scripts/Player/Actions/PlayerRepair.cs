using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerRepair : PlayerControl
{
    private int maxInteractDistance = 1;

    [SerializeField] private AudioHandler audioHandler;

    public override List<GridPosition> GetValidPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition playerGridPosition = playerChar.GetGridPosition();

        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = playerGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                DefenseHealthUI defense = LevelGrid.Instance.GetDefenseAtGridPosition(testGridPosition);

                if (defense == null)
                {
                    // No Defense on this GridPosition
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
    }

    public override void DoControl(GridPosition gridPosition, Action onActionComplete)
    {
        DefenseHealthUI defense = LevelGrid.Instance.GetDefenseAtGridPosition(gridPosition);

        if (defense != null)
        {
            audioHandler.PlayRepair();
            // Player character animator bool repair true 
            playerChar.RepairDefense();
            defense.Repair(OnRepairComplete);
        }

        ActionStart(onActionComplete);
    }

    private void OnRepairComplete()
    {
        playerChar.OnRepairComplete();
        ActionComplete();
        StartCoroutine(StopRepairSound());
    }

    private IEnumerator StopRepairSound()
    {
        yield return new WaitForSeconds(3f);
        audioHandler.StopRepair();
    }

}
