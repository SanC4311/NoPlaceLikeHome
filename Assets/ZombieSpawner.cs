using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject[] zombiePrefabs; // Array to hold zombie variants
    public Transform targetPosition;
    public DefenseHealth defenseHealth;
    public float stoppingDistance = 1f; // Distance to stop from the target
    public float spawnInterval = 1f; // Time between each spawn
    private float timer;

    void Start()
    {
        timer = spawnInterval + Random.Range(-2f, 20f); // Initialize the timer
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnZombie();
            timer = spawnInterval + Random.Range(-2f, 20f); // Reset timer with a random interval around spawnInterval
            Debug.Log($"Next zombie spawning in: {timer:F2} seconds"); // Log the next spawn time
        }
    }

    void SpawnZombie()
    {
        int zombieIndex = Random.Range(0, zombiePrefabs.Length); // Select a random zombie variant
        GameObject spawnedZombie = Instantiate(zombiePrefabs[zombieIndex], transform.position, Quaternion.identity); // Spawn the zombie

        // Pass the target position and defense health to the spawned zombie
        ZombieAI zombieAI = spawnedZombie.GetComponent<ZombieAI>();
        if (zombieAI != null)
        {
            zombieAI.Initialize(targetPosition, defenseHealth, stoppingDistance);
        }
    }
}
