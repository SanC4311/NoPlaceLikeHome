using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [SerializeField] private Animator zombieAnimator;
    public Transform targetPosition;
    //[SerializeField] private NavMeshAgent agent;
    public float moveSpeed = 3.2f;
    public float stoppingDistance;
    public float spawnInterval = 5f;
    private float timer;

    public LayerMask zombieLayer; // Layer to check for other zombies
    public float checkDistance = 0.2f; // Distance to check for other zombies in front

    private bool reachedTarget = false;

    public float attackRate = 2f; // How often the zombie attacks in seconds
    public float attackDamage = 10f; // Damage per attack
    private float lastAttackTime = 0f; // Time since last attack

    public DefenseHealth defenseHealth;
    public void Initialize(Transform target, DefenseHealth health, float StoppingDistance)
    {
        targetPosition = target;
        defenseHealth = health;
        stoppingDistance = StoppingDistance;
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnInterval)
        {
            SpawnZombie();
        }
    }

    public void SpawnZombie()
    {
        if (!reachedTarget)
        {
            MoveToTarget();
        }
        else
        {
            // Ensure the zombie stops moving
            zombieAnimator.SetBool("IsWalking", false);

            // Trigger attack animation
            zombieAnimator.SetBool("Attack", true);
            if (Time.time - lastAttackTime >= attackRate)
            {

                AttackTarget();
                lastAttackTime = Time.time;
            }
        }
    }

    public void MoveToTarget()
    {
        if (targetPosition != null)
        {
            // Create a new Vector3 that ignores the y component of the target position
            Vector3 targetPositionFlat = new Vector3(targetPosition.position.x, transform.position.y, targetPosition.position.z);

            bool canMove = !Physics.Raycast(transform.position, transform.forward, checkDistance, zombieLayer);
            //Debug.Log(canMove);

            if (Vector3.Distance(transform.position, targetPositionFlat) > stoppingDistance && canMove)
            {
                transform.LookAt(targetPositionFlat);
                transform.position = Vector3.MoveTowards(transform.position, targetPositionFlat, moveSpeed * Time.deltaTime);
                zombieAnimator.SetBool("IsWalking", true);
            }
            else if (Vector3.Distance(transform.position, targetPositionFlat) <= stoppingDistance)
            {
                reachedTarget = true;
                zombieAnimator.SetBool("IsWalking", false);
            }
            else
            {
                zombieAnimator.SetBool("IsWalking", false);
            }
        }
    }

    public void AttackTarget()
    {
        // Assuming the targetPosition is the door/window GameObject
        if (targetPosition != null)
        {
            DefenseHealth targetHealth = defenseHealth;
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage);
                // Debug.Log("Zombie attacked the target for " + attackDamage + " damage!");
                // Debug.Log("Target health: " + targetHealth.health);
            }
            if (targetHealth.targetDestroyed)
            {
                // The target has been destroyed
                Debug.Log("Target destroyed!");
                // Trigger a "destroyed" animation
                zombieAnimator.SetBool("Attack", false);
                zombieAnimator.SetBool("Destroyed", true);
                Destroy(gameObject, 4f);
            }
        }
    }
}
