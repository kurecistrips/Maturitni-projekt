
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab; // The enemy prefab to spawn
    public int count = 1; // How many enemies of this type to spawn
    public float timeBetweenSpawns = 0.5f; // Time between spawns within this batch
    public float spawnAfterStartTime = 0f; // Time after wave starts to spawn this batch
}

[System.Serializable]
public class Wave
{
    public List<EnemySpawnInfo> enemies; // List of enemy batches in this wave
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager main;
    public List<Wave> waves; // List of waves
    public Transform spawnPoint; // Where enemies will spawn
    public float timeBetweenWaves = 5f; // Time between two waves

    private int currentWaveIndex = 0; // Track the current wave
    public int waveIndex = 1;
    public float waveStartTime; // Time when the current wave started
    private int enemiesLeftToSpawn; // Track how many enemies are left to spawn in the current wave
    private int remainingEnemiesDebug; // How many enemies are remaining from the wave (resets when a new wave starts (might change it later))

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    // Coroutine to spawn waves
    private IEnumerator SpawnWaves() // Problem with delay timer and UI timer, they are out of sync
    {
        while (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];

            // Calculate the total number of enemies for the wave
            enemiesLeftToSpawn = CalculateTotalEnemiesInWave(currentWave);

            // Log and optionally display enemy counts by type
            Dictionary<GameObject, int> enemyCounts = CountEnemiesByType(currentWave);
            LogEnemyCounts(currentWaveIndex + 1, enemyCounts);
            //DisplayEnemyCounts(enemyCounts);

            // Record the start time of this wave
            waveStartTime = Time.time;

            // Spawn enemies for the wave
            yield return StartCoroutine(SpawnEnemiesInWave(currentWave));

            // After the wave completes, wait before starting the next one
            yield return new WaitForSeconds(timeBetweenWaves);

            currentWaveIndex++;
            waveIndex++;
        }

        Debug.Log("All waves completed!");
    }

    // Coroutine to spawn enemies in a wave
    private IEnumerator SpawnEnemiesInWave(Wave wave)
    {
        for (int i = 0; i < wave.enemies.Count; i++)
        {
            EnemySpawnInfo enemyInfo = wave.enemies[i];

            // Wait until the specified time after the wave start
            float targetSpawnTime = waveStartTime + enemyInfo.spawnAfterStartTime;
            yield return new WaitUntil(() => Time.time >= targetSpawnTime);

            // Spawn the specified number of enemies for this batch
            for (int j = 0; j < enemyInfo.count; j++)
            {
                // Spawn the enemy
                Instantiate(enemyInfo.enemyPrefab, spawnPoint.position, Quaternion.identity);

                // Decrease the remaining enemies count
                enemiesLeftToSpawn--;
                remainingEnemiesDebug++;

                // Log and update UI
                Debug.Log($"Spawned enemy. Remaining enemies in wave: {enemiesLeftToSpawn}");
                //UpdateRemainingEnemiesText();

                // If it's not the last enemy in the batch, wait between spawns
                if (j < enemyInfo.count - 1)
                {
                    yield return new WaitForSeconds(enemyInfo.timeBetweenSpawns);
                }
            }
        }
    }

    // Calculate the total number of enemies in a wave
    private int CalculateTotalEnemiesInWave(Wave wave)
    {
        int totalEnemies = 0;
        remainingEnemiesDebug = 0;

        foreach (EnemySpawnInfo enemyInfo in wave.enemies)
        {
            totalEnemies += enemyInfo.count;
        }

        return totalEnemies;
    }

    // Count how many enemies of each type are in a wave
    private Dictionary<GameObject, int> CountEnemiesByType(Wave wave)
    {
        Dictionary<GameObject, int> enemyTypeCounts = new Dictionary<GameObject, int>();

        foreach (EnemySpawnInfo enemyInfo in wave.enemies)
        {
            if (enemyTypeCounts.ContainsKey(enemyInfo.enemyPrefab))
            {
                enemyTypeCounts[enemyInfo.enemyPrefab] += enemyInfo.count;
            }
            else
            {
                enemyTypeCounts[enemyInfo.enemyPrefab] = enemyInfo.count;
            }
        }

        return enemyTypeCounts;
    }

    // Log enemy type counts to the console
    private void LogEnemyCounts(int waveNumber, Dictionary<GameObject, int> enemyCounts)
    {
        Debug.Log($"Wave {waveNumber} enemy counts:");
        foreach (KeyValuePair<GameObject, int> pair in enemyCounts)
        {
            Debug.Log($"{pair.Value} enemies of type {pair.Key.name}");
        }
    }
    public float GetTimeUntilNextWave()
    {
        float nextWaveStartTime = waveStartTime + timeBetweenWaves;
        return Mathf.Max(0, nextWaveStartTime - Time.time);
    }
}
