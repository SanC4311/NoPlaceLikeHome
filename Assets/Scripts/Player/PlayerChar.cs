using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;


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
    public bool playerTurned = false;
    public bool isShootingInProgress = false;
    public int startingBullets = 20;
    public int bullets;
    public bool outOfAmmo = false;
    [SerializeField] private GameObject bulletProjectilePrefab;
    [SerializeField] private GameObject bulletFX;
    [SerializeField] private GameObject shellsFX;
    [SerializeField] private GameObject bloodFX;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform originalParent;
    [SerializeField] private Animator PlayerCharAnimator;
    [SerializeField] private AudioHandler audioHandler;
    public bool validWindowPosition = false;
    public bool isRepairing = false;
    public float bulletFXTime = 0.1f;
    public float shellsFXTime = 1f;
    float tolerance = 0.5f;

    private enum wallType
    {
        front,
        back,
        left,
        right
    }
    wallType wall = wallType.front;

    public event EventHandler<OnAmmoChangedEventArgs> OnAmmoChanged;
    public class OnAmmoChangedEventArgs : EventArgs
    {
        public int Ammo;
    }

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

        bullets = startingBullets;
        OnAmmoChanged?.Invoke(this, new OnAmmoChangedEventArgs
        {
            Ammo = bullets
        });
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance.PlayerCharMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }

        // these are windows - 1,4 and 3,4. Enable attack mode (which also turns the player)
        if (((gridPosition.x >= 1 - tolerance && gridPosition.x <= 1 + tolerance) &&
            (gridPosition.z >= 4 - tolerance && gridPosition.z <= 4 + tolerance)) ||
            ((gridPosition.x >= 3 - tolerance && gridPosition.x <= 3 + tolerance) &&
            (gridPosition.z >= 4 - tolerance && gridPosition.z <= 4 + tolerance)))
        {
            wall = wallType.front;
            if (playerTurned)
            {
                playerTurned = false;
            }
            validWindowPosition = true;
            EnableAttackMode();
        }
        // this is a door - 2,4. Turn player only, and do not enable attack mode
        else if ((gridPosition.x >= 2 - tolerance && gridPosition.x <= 2 + tolerance) &&
            (gridPosition.z >= 4 - tolerance && gridPosition.z <= 4 + tolerance))
        {
            wall = wallType.front;
            validWindowPosition = false;
            attackMode = false;
            if (!playerTurned)
            {

                Quaternion targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
                StartCoroutine(TurnComplete());
            }
        }
        else if ((gridPosition.x >= 2 - tolerance && gridPosition.x <= 2 + tolerance) &&
            (gridPosition.z >= 0 - tolerance && gridPosition.z <= 0 + tolerance))
        {
            wall = wallType.back;
            if (playerTurned)
            {
                playerTurned = false;
            }
            validWindowPosition = true;
            EnableAttackMode();
        }
        else if ((gridPosition.x >= 3 - tolerance && gridPosition.x <= 3 + tolerance) &&
                    (gridPosition.z >= 0 - tolerance && gridPosition.z <= 0 + tolerance))
        {
            wall = wallType.back;
            validWindowPosition = false;
            attackMode = false;
            if (!playerTurned)
            {
                Quaternion targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 180, transform.rotation.eulerAngles.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
                StartCoroutine(TurnComplete());
            }
        }
        else if (((gridPosition.x >= 0 - tolerance && gridPosition.x <= 0 + tolerance) &&
            (gridPosition.z >= 1 - tolerance && gridPosition.z <= 1 + tolerance)) ||
            ((gridPosition.x >= 0 - tolerance && gridPosition.x <= 0 + tolerance) &&
            (gridPosition.z >= 3 - tolerance && gridPosition.z <= 3 + tolerance)))
        {
            wall = wallType.left;
            if (playerTurned)
            {
                playerTurned = false;
            }
            validWindowPosition = true;
            EnableAttackMode();
        }
        else if ((gridPosition.x >= 4 - tolerance && gridPosition.x <= 4 + tolerance) &&
            (gridPosition.z >= 1 - tolerance && gridPosition.z <= 1 + tolerance))
        {
            wall = wallType.right;
            if (playerTurned)
            {
                playerTurned = false;
            }
            validWindowPosition = true;
            EnableAttackMode();
        }
        else if (attackMode)
        {
            attackMode = false;
            isShootingInProgress = false;
            PlayerCharAnimator.SetBool("isAiming", false);
            playerTurned = false;
            validWindowPosition = false;
        }
        else if (playerTurned)
        {
            playerTurned = false;
            validWindowPosition = false;
        }

        if (bullets <= 0)
        {
            outOfAmmo = true;
        }
        else
        {
            outOfAmmo = false;
        }

        // Debug.Log("bullets: " + bullets);
        // Debug.Log("bullets outOfAmmo: " + outOfAmmo);
    }

    IEnumerator TurnComplete()
    {
        yield return new WaitForSeconds(0.6f);
        playerTurned = true;
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
        if (outOfAmmo)
        {
            Debug.Log("Out of ammo, cannot enable attack mode.");
            return;
        }
        if (validWindowPosition && !isRepairing)
        {
            if (!playerTurned)
            {
                Quaternion targetRotation = Quaternion.identity;
                if (wall == wallType.front)
                {
                    targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0, transform.rotation.eulerAngles.z);
                }
                else if (wall == wallType.back)
                {
                    targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 180, transform.rotation.eulerAngles.z);
                }
                else if (wall == wallType.left)
                {
                    targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, -90, transform.rotation.eulerAngles.z);
                }
                else if (wall == wallType.right)
                {
                    targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 90, transform.rotation.eulerAngles.z);
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }
            if (!attackMode)
            {
                Debug.Log("Attack mode enabled.");
                if (outOfAmmo)
                {
                    Debug.Log("Out of ammo, cannot enable attack mode.");
                    return;
                }
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
                validWindowPosition = false;
            }
        }
    }

    IEnumerator AimAndShoot()
    {
        isShootingInProgress = true;

        if (isRepairing || !validWindowPosition || outOfAmmo)
        {
            playerTurned = false;
            PlayerCharAnimator.SetBool("isAiming", false);
            isShootingInProgress = false;
            attackMode = false;
            yield break;
        }

        while (attackMode && !isRepairing && validWindowPosition && !outOfAmmo)
        {
            PlayerCharAnimator.SetBool("isAiming", true);
            List<Transform> zombiesInSight = new List<Transform>();

            if (isRepairing || !validWindowPosition || outOfAmmo)
            {
                playerTurned = false;
                PlayerCharAnimator.SetBool("isAiming", false);
                isShootingInProgress = false;
                attackMode = false;
                yield break;
            }

            // Populate zombiesInSight with detected zombies
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

            // Now sort zombiesInSight by distance from the survivor
            zombiesInSight = zombiesInSight.OrderBy(
                z => (z.position - transform.position).sqrMagnitude).ToList();

            if (zombiesInSight.Count > 0 && !isRepairing && validWindowPosition && !outOfAmmo)
            {
                Debug.Log("Preparing to shoot zombies.");
                yield return StartCoroutine(ShootZombies(zombiesInSight));

                if (isRepairing || !validWindowPosition || outOfAmmo)
                {
                    playerTurned = false;
                    PlayerCharAnimator.SetBool("isAiming", false);
                    isShootingInProgress = false;
                    attackMode = false;
                    yield break;
                }
            }
            else
            {
                if (isRepairing || !validWindowPosition || outOfAmmo)
                {
                    playerTurned = false;
                    PlayerCharAnimator.SetBool("isAiming", false);
                    isShootingInProgress = false;
                    attackMode = false;
                    yield break;
                }
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
        if (isRepairing || !validWindowPosition || outOfAmmo)
        {
            playerTurned = false;
            isShootingInProgress = false;
            attackMode = false;
            yield break;
        }

        yield return new WaitForSeconds(2); // Wait for aim animation to complete
        playerTurned = true;

        if (isRepairing || !validWindowPosition || outOfAmmo)
        {
            playerTurned = false;
            isShootingInProgress = false;
            attackMode = false;
            yield break;
        }

        foreach (Transform zombie in zombies)
        {
            if (zombie != null && !isRepairing && validWindowPosition && !outOfAmmo) // Check if the zombie still exists
            {
                // if zombie is destroyed, step to next zombie
                if (zombie.GetComponent<ZombieAI>().isDestroyed)
                {
                    continue;
                }
                Debug.Log($"Targeting zombie at {zombie.position}.");

                // Face the zombie
                Vector3 directionToZombie = zombie.position - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToZombie.x, 0, directionToZombie.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f); // Immediate rotation for simplicity

                if (isRepairing || !validWindowPosition || outOfAmmo)
                {
                    playerTurned = false;
                    isShootingInProgress = false;
                    attackMode = false;
                    yield break;
                }
                yield return new WaitForSeconds(2); // Wait for aim animation to complete
                // Simulate aiming at the zombie
                Debug.Log("Aimed at zombie, preparing to shoot.");

                if (isRepairing || !validWindowPosition || outOfAmmo)
                {
                    playerTurned = false;
                    isShootingInProgress = false;
                    attackMode = false;
                    yield break;
                }

                // Shoot the zombie
                PlayerCharAnimator.SetBool("isShooting", true);
                yield return new WaitForSeconds(0.1f); // Ensure animation starts

                if (isRepairing || !validWindowPosition || outOfAmmo)
                {
                    playerTurned = false;
                    isShootingInProgress = false;
                    attackMode = false;
                    yield break;
                }

                shootPoint.SetParent(playerRoot, false);

                // Debug.Log("Local: " + shootPoint.localPosition);
                // Debug.Log("Global: " + shootPoint.position);

                Vector3 bulletPosition = new Vector3((float)(shootPoint.position.x + 0.117), (float)(shootPoint.position.y + 1.315 - 0.082), (float)(shootPoint.position.z + 0.023));
                GameObject trailEffect = Instantiate(bulletProjectilePrefab, bulletPosition, quaternion.identity);
                Debug.Log("Bullet shot.");
                GameObject shootingEffect = Instantiate(bulletFX, bulletPosition, quaternion.identity);
                Quaternion shellsRotation = Quaternion.Euler(0, 0, -90);
                GameObject shellsEffect = Instantiate(shellsFX, bulletPosition, shellsRotation);

                if (isRepairing || !validWindowPosition || outOfAmmo)
                {
                    Destroy(shootingEffect);
                    Destroy(shellsEffect);
                    playerTurned = false;
                    isShootingInProgress = false;
                    attackMode = false;
                    yield break;
                }

                audioHandler.PlayGunshot();

                ParticleSystem ps = shootingEffect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                    Debug.Log("Bullet FX Started.");
                }
                Debug.Log("Bullet FX Done.");

                ParticleSystem psShells = shellsEffect.GetComponent<ParticleSystem>();
                if (psShells != null)
                {
                    psShells.Play();
                    Debug.Log("Shells FX Started.");
                }

                GameObject bloodEffect = new GameObject();

                BulletProjectile bulletProjectile = trailEffect.GetComponent<BulletProjectile>();
                if (zombie != null)
                {
                    bullets = bullets - 1;
                    OnAmmoChanged?.Invoke(this, new OnAmmoChangedEventArgs
                    {
                        Ammo = bullets
                    });
                    Vector3 shotPosition = new Vector3(zombie.position.x, (float)(zombie.position.y + 1.315), zombie.position.z);
                    bulletProjectile.Setup(shotPosition);
                    bloodEffect = Instantiate(bloodFX, shotPosition, quaternion.identity);
                }
                shootPoint.SetParent(originalParent, false);

                ParticleSystem psBlood = bloodEffect.GetComponent<ParticleSystem>();
                if (psBlood != null)
                {
                    psBlood.Play();
                    Debug.Log("Blood FX Started.");
                }

                if (isRepairing || !validWindowPosition || outOfAmmo)
                {
                    Destroy(shootingEffect);
                    Destroy(shellsEffect);
                    Destroy(bloodEffect);
                    playerTurned = false;
                    isShootingInProgress = false;
                    attackMode = false;
                    yield break;
                }

                //yield return new WaitForSeconds(2); // Duration for shooting animation

                // Destroy the zombie
                if (zombie != null)
                {
                    Debug.Log("Zombie shot and destroying now.");
                    zombie.GetComponent<ZombieAI>().isDestroyed = true;
                    zombie.GetComponent<ZombieAI>().DeathSound();
                    if (isRepairing || !validWindowPosition || outOfAmmo)
                    {
                        Destroy(shootingEffect);
                        Destroy(shellsEffect);
                        Destroy(bloodEffect);
                        playerTurned = false;
                        isShootingInProgress = false;
                        attackMode = false;
                        yield break;
                    }
                }
                ScreenShake.Instance.ShakeCamera(2.5f);
                yield return new WaitForSeconds(bulletFXTime);
                Debug.Log("Bullet FX Done. Destroying effect.");
                Destroy(shootingEffect);
                yield return new WaitForSeconds(shellsFXTime);
                Destroy(shellsEffect);
                yield return new WaitForSeconds(1);
                Destroy(bloodEffect);
                audioHandler.StopGunshot();

                PlayerCharAnimator.SetBool("isShooting", false);
                if (isRepairing || !validWindowPosition || outOfAmmo)
                {
                    playerTurned = false;
                    isShootingInProgress = false;
                    attackMode = false;
                    yield break;
                }
                yield return new WaitForSeconds(1); // Delay before targeting the next zombie
            }
        }

        // After finishing shooting all zombies:
        isShootingInProgress = false;
        PlayerCharAnimator.SetBool("isAiming", false);
        Debug.Log("All targeted zombies have been shot.");
        playerTurned = false;
    }

    public void RepairDefense()
    {
        Debug.Log("isRepairing = true.");
        isRepairing = true;
        PlayerCharAnimator.SetBool("isRepairing", true);
    }

    public void OnRepairComplete()
    {
        StartCoroutine(RepairComplete());
    }

    IEnumerator RepairComplete()
    {
        yield return new WaitForSeconds(3);
        Debug.Log("Repair complete.");
        PlayerCharAnimator.SetBool("isRepairing", false);
        Debug.Log("isRepairing = false.");
        isRepairing = false;
    }

    public void reloadBullets()
    {
        bullets = startingBullets;
        outOfAmmo = false;
        OnAmmoChanged?.Invoke(this, new OnAmmoChangedEventArgs
        {
            Ammo = bullets
        });

        PlayerCharAnimator.SetBool("isReloading", true);
    }

    public void OnReloadComplete()
    {
        StartCoroutine(ReloadComplete());
    }

    IEnumerator ReloadComplete()
    {
        yield return new WaitForSeconds(2);
        PlayerCharAnimator.SetBool("isReloading", false);
    }


}
