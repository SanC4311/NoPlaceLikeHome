using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 4f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private Animator playerCharAnimator;
    [SerializeField] private int maxMoveDistance = 4;
    private Vector3 targetPosition;
    private PlayerChar playerChar;
    private void Awake()
    {
        playerChar = GetComponent<PlayerChar>();
        targetPosition = transform.position;
    }

    private void Update()
    {
        float stoppingDistance = 0.1f;

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            Vector3 directMovement = transform.forward;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);

            playerCharAnimator.SetBool("IsWalking", true);
        }
        else
        {
            playerCharAnimator.SetBool("IsWalking", false);
        }
    }

    public void Move(GridPosition gridPosition)
    {
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
    }

    public bool IsValidPosition(GridPosition gridPosition)
    {
        List<GridPosition> validPositionList = GetValidPositionList();
        return validPositionList.Contains(gridPosition);
    }

    public List<GridPosition> GetValidPositionList()
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
