using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifler : MonoBehaviour
{
    [SerializeField] private Animator riflerAnimator;
    [SerializeField] private float rotateSpeed = 4f;
    [SerializeField] private float moveSpeed = 4f;
    private Vector3 targetPosition;

    private void Awake()
    {
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

            riflerAnimator.SetBool("IsWalking", true);
        }
        else
        {
            riflerAnimator.SetBool("IsWalking", false);
        }
    }

    public void Move(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

}
