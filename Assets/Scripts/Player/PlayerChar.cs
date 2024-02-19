using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerChar : MonoBehaviour
{
    private GridPosition gridPosition;
    private PlayerMove playerMove;
    //private PlayerDefendRifle playerDefend;
    private PlayerControl[] playerControls;

    public bool attackMode = false;
    private float rotateSpeed = 6f;
    [SerializeField] private LayerMask zombieLayer; // Assign this in the inspector
    public float shootingRange = 10f;
    public float shootingAngle = 45f;
    private bool playerTurned = false;
    private bool isShootingInProgress = false;

    [SerializeField] private GameObject bulletProjectilePrefab;
    [SerializeField] private GameObject bulletFX;
    [SerializeField] private Transform shootPoint;

    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform originalParent;
    [SerializeField] private Animator PlayerCharAnimator;
    public float bulletFXTime = 0.1f;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        //playerDefend = GetComponent<PlayerDefendRifle>();
        playerControls = GetComponents<PlayerControl>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddPlayerCharAtGridPosition(gridPosition, this);
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance.PlayerCharMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
        EnableAttackMode();
    }

    public PlayerMove GetPlayerMove()
    {
        return playerMove;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public PlayerControl[] GetPlayerControls()
    {
        return playerControls;
    }

    public void EnableAttackMode()
    {
        float tolerance = 0.5f;
        if ((gridPosition.x >= 1 - tolerance && gridPosition.x <= 1 + tolerance) &&
            (gridPosition.z >= 4 - tolerance && gridPosition.z <= 4 + tolerance) ||
            ((gridPosition.x >= 3 - tolerance && gridPosition.x <= 3 + tolerance) &&
            (gridPosition.z >= 4 - tolerance && gridPosition.z <= 4 + tolerance)))
        {
            if (!playerTurned)
            {
                Quaternion targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }
            if (!attackMode)
            {
                Debug.Log("Attack mode enabled.");
                attackMode = true;
                if (!isShootingInProgress)
                {
                    StartCoroutine(AimAndShoot());
                }
            }
        }
        else
        {
            if (attackMode)
            {
                Debug.Log("Attack mode disabled.");
                attackMode = false;
                // Stop the AimAndShoot coroutine if it's running
                StopCoroutine(AimAndShoot());
                isShootingInProgress = false;
                PlayerCharAnimator.SetBool("isAiming", false);
                playerTurned = false;
            }
        }
    }

    IEnumerator AimAndShoot()
    {
        isShootingInProgress = true;

        while (attackMode)
        {
            PlayerCharAnimator.SetBool("isAiming", true);
            List<Transform> zombiesInSight = new List<Transform>();

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, shootingRange, zombieLayer);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider != null)
                {
                    Vector3 directionToZombie = hitCollider.transform.position - transform.position;
                    if (Vector3.Angle(transform.forward, directionToZombie) <= shootingAngle / 2)
                    {
                        zombiesInSight.Add(hitCollider.transform);
                        Debug.Log("Zombie spotted.");
                    }
                }
            }

            if (zombiesInSight.Count > 0)
            {
                Debug.Log("Preparing to shoot zombies.");
                yield return StartCoroutine(ShootZombies(zombiesInSight));
            }
            else
            {
                Debug.Log("No zombies in sight.");
                PlayerCharAnimator.SetBool("isAiming", false);
                // Wait a moment before checking again
                yield return new WaitForSeconds(1f);
            }
        }

        isShootingInProgress = false;
    }

    IEnumerator ShootZombies(List<Transform> zombies)
    {
        yield return new WaitForSeconds(2); // Wait for aim animation to complete
        playerTurned = true;

        foreach (Transform zombie in zombies)
        {
            if (zombie != null) // Check if the zombie still exists
            {
                Debug.Log($"Targeting zombie at {zombie.position}.");

                // Face the zombie
                Vector3 directionToZombie = zombie.position - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToZombie.x, 0, directionToZombie.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f); // Immediate rotation for simplicity

                // Simulate aiming at the zombie
                yield return new WaitForSeconds(1f);
                Debug.Log("Aimed at zombie, preparing to shoot.");

                // Shoot the zombie
                PlayerCharAnimator.SetBool("isShooting", true);
                yield return new WaitForSeconds(0.1f); // Ensure animation starts

                shootPoint.SetParent(playerRoot, false);

                Debug.Log("Local: " + shootPoint.localPosition);
                Debug.Log("Global: " + shootPoint.position);

                Vector3 bulletPosition = new Vector3((float)(shootPoint.position.x + 0.117), (float)(shootPoint.position.y + 1.315 - 0.082), (float)(shootPoint.position.z + 0.023));
                GameObject trailEffect = Instantiate(bulletProjectilePrefab, bulletPosition, quaternion.identity);
                Debug.Log("Bullet shot.");
                GameObject shootingEffect = Instantiate(bulletFX, bulletPosition, quaternion.identity);

                ParticleSystem ps = shootingEffect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                    Debug.Log("Bullet FX Started.");
                }
                Debug.Log("Bullet FX Done.");
                BulletProjectile bulletProjectile = trailEffect.GetComponent<BulletProjectile>();
                if (zombie != null)
                {
                    Vector3 headshotPosition = new Vector3(zombie.position.x, (float)(zombie.position.y + 1.315), zombie.position.z);
                    bulletProjectile.Setup(headshotPosition);
                }
                shootPoint.SetParent(originalParent, false);

                //yield return new WaitForSeconds(2); // Duration for shooting animation

                // Destroy the zombie
                if (zombie != null)
                {
                    Debug.Log("Zombie shot and destroying now.");
                    zombie.GetComponent<ZombieAI>().isDestroyed = true;
                    yield return new WaitForSeconds(bulletFXTime);
                    Debug.Log("Bullet FX Done. Destroying effect.");
                    Destroy(shootingEffect);
                }

                PlayerCharAnimator.SetBool("isShooting", false);
                yield return new WaitForSeconds(1); // Delay before targeting the next zombie
            }
        }

        // After finishing shooting all zombies:
        isShootingInProgress = false;
        PlayerCharAnimator.SetBool("isAiming", false);
        Debug.Log("All targeted zombies have been shot.");
        playerTurned = false;
    }

}
