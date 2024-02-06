using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMove : PlayerControl
{
    [SerializeField] private float rotateSpeed = 3f;
    [SerializeField] private float moveSpeed = 3.2f;
    [SerializeField] private Animator playerCharAnimator;
    [SerializeField] private int maxMoveDistance = 4;
    private Vector3 targetPosition;
    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float stoppingDistance = 0.1f;

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // Set IsWalking to true
            playerCharAnimator.SetBool("IsWalking", true);
        }
        else
        {
            // Set IsWalking to false
            playerCharAnimator.SetBool("IsWalking", false);
            isActive = false;
            onActionComplete();
        }

        // Adjust rotation to only consider the y-axis
        Vector3 horizontalMoveDirection = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;
        if (horizontalMoveDirection != Vector3.zero) // Check to avoid setting a zero rotation
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalMoveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    public override void DoControl(GridPosition gridPosition, Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        isActive = true;
    }

    public override List<GridPosition> GetValidPositionList()
    {
        List<GridPosition> validPositionList = new List<GridPosition>();

        GridPosition playerCharGridPosition = playerChar.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = playerCharGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (playerCharGridPosition == testGridPosition)
                {
                    continue;
                }

                if (LevelGrid.Instance.HasAnyPlayerCharAtGridPosition(testGridPosition))
                {
                    continue;
                }

                validPositionList.Add(testGridPosition);
            }
        }

        return validPositionList;
    }
}
