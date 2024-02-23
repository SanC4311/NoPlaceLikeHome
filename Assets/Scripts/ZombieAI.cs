using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [SerializeField] private Animator zombieAnimator;
    public Transform targetPosition;
    public float moveSpeed = 3.2f;
    public float stoppingDistance;
    public float spawnInterval = 4f;
    private float timer;

    public LayerMask zombieLayer; // Layer to check for other zombies
    public float checkDistance = 0.2f; // Distance to check for other zombies in front

    private bool reachedTarget = false;
    public float attackRate = 5f; // How often the zombie attacks in seconds
    public float attackDamage = 5f; // Damage per attack
    private float lastAttackTime = 0f; // Time since last attack

    public bool isDestroyed = false;
    public bool playDeathSound = false;
    [SerializeField] private AudioHandler audioHandler;

    public AudioSource footsSource;
    public AudioSource deathSource;

    public DefenseHealth defenseHealth;
    public void Initialize(Transform target, DefenseHealth health, float StoppingDistance)
    {
        targetPosition = target;
        defenseHealth = health;
        stoppingDistance = StoppingDistance;
        audioHandler.PlaySpawn();
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnInterval)
        {
            SpawnZombie();
        }

        Debug.Log("isDestroyed: " + isDestroyed);

        if (isDestroyed)
        {
            // The target has been destroyed
            Debug.Log("Zombie destroyed - Script!");
            // Trigger a "destroyed" animation
            zombieAnimator.SetBool("Destroyed", true);
            Destroy(gameObject, 4f); // Destroy the zombie after 2 seconds
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
                audioHandler.PlayGunshot();
                AttackTarget();
                lastAttackTime = Time.time;
                StartCoroutine(StopAttackSound());
            }
        }
    }

    public void DeathSound()
    {
        Debug.Log("Playing death sound - newest ");
        deathSource.pitch = UnityEngine.Random.Range(0.7f, 1.4f);
        deathSource.Play();
    }

    private IEnumerator StopAttackSound()
    {
        yield return new WaitForSeconds(2f);
        audioHandler.StopGunshot();
    }

    public void MoveToTarget()
    {
        if (targetPosition != null)
        {
            // Create a new Vector3 that ignores the y component of the target position
            Vector3 targetPositionFlat = new Vector3(targetPosition.position.x, transform.position.y, targetPosition.position.z);

            bool canMove = !Physics.Raycast(transform.position, transform.forward, checkDistance, zombieLayer);
            Debug.Log(canMove);

            if (canMove)
            {
                Debug.Log("CanMove so playing sound");
                if (!footsSource.isPlaying)
                {
                    audioHandler.PlayFootsteps(0.8f, 1.5f);
                }
            }

            if (Vector3.Distance(transform.position, targetPositionFlat) > stoppingDistance && canMove && !isDestroyed)
            {
                transform.LookAt(targetPositionFlat);
                transform.position = Vector3.MoveTowards(transform.position, targetPositionFlat, moveSpeed * Time.deltaTime);
                zombieAnimator.SetBool("IsWalking", true);
            }
            else if (Vector3.Distance(transform.position, targetPositionFlat) <= stoppingDistance)
            {
                audioHandler.StopFootsteps();
                reachedTarget = true;
                zombieAnimator.SetBool("IsWalking", false);
            }
            else
            {
                audioHandler.StopFootsteps();
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
