using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]

public class EnemySpawnInfoTest
{
    public GameObject enemyPrefab;
    public int count = 1;
    public bool hidden = false;
    public float timeBetweenSpawns = 0.5f;
    public float spawnAfterStartTime = 0f;
}

[System.Serializable]

public class WaveTest
{
    public List<EnemySpawnInfoTest> enemies;
    public float timeBetweenWaves;
    public int currencyPerWave;
}

public class WaveManagerTest : MonoBehaviour
{
    public static WaveManagerTest main;
    public List<WaveTest> waves;
    public Transform spawnPoint;
    public GameOutcome gameOutcome;

    public static UnityEvent onEnemyDestroy = new UnityEvent();

    private int currentWaveIndex = 0;
    public int waveIndex = 1;
    public float waveStartTimeDebug;
    private float intermissionTime = 5f;
    private float timeSinceWaveStart;
    private float preptime = 20f;
    private bool prepBool = true;
    private float waveTimeLength;

    private int enemiesLeftToSpawn;
    private int enemiesAlive;
    public bool waveCommencing = false;
    [SerializeField] private int getCoins = 100;
    private float victoryBonus = 0.5f;
    public int showReceiveCoins;
    private float recievedCoins;
    private float maxAmountOfWaves;
    private bool rewardGiven = false;

    private bool currencyGiven = false;
    private bool skipBool = false;

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
        main = this;
    }

    private void Start()
    {
        waveStartTimeDebug = Time.time;
        maxAmountOfWaves = waves.Count;
    }

    private void Update()
    {
        timeSinceWaveStart += Time.deltaTime;
        

        if (prepBool)
        {
            PrepTime();
        }        
        else
        {
            preptime += 0;
            
            if (currentWaveIndex < waves.Count)
            {
                if (waveCommencing == true && currentWaveIndex == 0)
                {
                    waveTimeLength = waves[currentWaveIndex].timeBetweenWaves;
                    SpawnWave();
                    waveIndex++;
                    
                }
                else if (waveTimeLength <= 0 || enemiesAlive == 0 && enemiesLeftToSpawn <= 0 && skipBool == false && LevelManager.main.gameEnded == false)
                {
                    
                    Intermission();
                    
                }
                else if (skipBool == true) 
                {
                    Intermission();
                }
                
            }
            else if(enemiesAlive <= 0 && enemiesLeftToSpawn == 0)
            {
                ScoreScreen();
            }
            waveTimeLength -= Time.deltaTime;
            showSkipPopUp();
            
        }
    }

    public void showSkipPopUp()
    {
        
        if (waveTimeLength <= waves[waveIndex-2].timeBetweenWaves * 0.90 && waveCommencing == true && prepBool == false && currentWaveIndex <= waves.Count-1 && SkipWaveScript.main.vetoed == false)
        {
            SkipWaveScript.main.skipPopUp.SetActive(true);
            
        }
        else {
            SkipWaveScript.main.skipPopUp.SetActive(false);
        }
    }

    public void SkipWave()
    {
        skipBool = true;
        Intermission();
        
    }

    private void PrepTime()
    {
        preptime -= Time.deltaTime;
        if (preptime <= 0)
        {
            prepBool = false;
            waveCommencing = true;
        }
    }
    
    private void Intermission()
    {
        EnemyMovement[] activeEnemies = FindObjectsOfType<EnemyMovement>();
        foreach (EnemyMovement enemy in activeEnemies)
        {
            enemy.fromLastWave = true;
        }

        waveCommencing = false;

        if (!currencyGiven)
        {
            LevelManager.main.IncreaseCurrency(waves[waveIndex-2].currencyPerWave);
            currencyGiven = true;
            BonusMoneyPopUp.main.MoneyPerWave(waves[waveIndex-2].currencyPerWave);
        }
        
        enemiesAlive = 0;
        intermissionTime -= Time.deltaTime;
        if (intermissionTime <= 0)
        {
            SkipWaveScript.main.vetoed = false;
            skipBool = false;
            waveIndex++;
            waveTimeLength = waves[currentWaveIndex].timeBetweenWaves;
            currencyGiven = false;
            SpawnWave();
            intermissionTime = 5f;
        }
    }

    private void SpawnWave()
    {
        
        if (currentWaveIndex >= waves.Count) return;

        Debug.Log($"wave: {waveIndex} is spawning");

        WaveTest currentWave = waves[currentWaveIndex];

        enemiesLeftToSpawn = CalculateTotalEnemiesInWave(currentWave);

        Dictionary<GameObject, int> enemyCounts = CountEnemiesByType(currentWave);
        LogEnemyCounts(currentWaveIndex + 1, enemyCounts);

        waveStartTimeDebug = Time.time;

        StartCoroutine(SpawnEnemiesInWave(currentWave));

        currentWaveIndex++;
        
        waveCommencing = true;
    }

    public void ScoreScreen()
    {
        gameOutcome.Activate();
        waveCommencing = false;
        recievedCoins = getCoins * (currentWaveIndex / maxAmountOfWaves);
        showReceiveCoins = (int)recievedCoins;
        
        if (!rewardGiven)
        {
            if (LevelManager.main.victory == true)
            {
                recievedCoins += recievedCoins*victoryBonus;
                
            }
            CurrencyManager.main.AddCurrency((int)recievedCoins);
            rewardGiven = true;
        }
        LevelManager.main.gameEnded = true;
        
    }

    private IEnumerator SpawnEnemiesInWave(WaveTest wave)
    {
        for (int i = 0; i < wave.enemies.Count; i++)
        {
            EnemySpawnInfoTest enemyInfo = wave.enemies[i];

            float enemySpawnTime = waveStartTimeDebug + enemyInfo.spawnAfterStartTime;
            yield return new WaitUntil(() => Time.time >= enemySpawnTime);

            for (int j = 0; j < enemyInfo.count; j++)
            {
                Instantiate(enemyInfo.enemyPrefab, spawnPoint.position, Quaternion.identity);
                if (enemyInfo.hidden == true) EnemyHealth.main.isHidden = true;
                
                enemiesLeftToSpawn--;
                enemiesAlive++;

                Debug.Log($"Spawned enemy. Remaining enemies in wave: {enemiesLeftToSpawn}");

                if (j < enemyInfo.count - 1)
                {
                    yield return new WaitForSeconds(enemyInfo.timeBetweenSpawns);
                }
            }
        }
    }

    private int CalculateTotalEnemiesInWave(WaveTest wave)
    {
        int totalEnemies = 0;
        enemiesAlive = 0;

        foreach (EnemySpawnInfoTest enemyInfo in wave.enemies)
        {
            totalEnemies += enemyInfo.count;
        }
        return totalEnemies;
    }

    private Dictionary<GameObject, int> CountEnemiesByType(WaveTest wave)
    {
        Dictionary<GameObject, int> enemyTypeCounts = new Dictionary<GameObject, int>();

        foreach (EnemySpawnInfoTest enemyInfo in wave.enemies)
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

    private void LogEnemyCounts(int waveNum, Dictionary<GameObject, int> enemyCounts)
    {
        Debug.Log($"Wave {waveNum} enemy counts:");
        foreach (KeyValuePair<GameObject, int> pair in enemyCounts)
        {
            Debug.Log($"{pair.Value} enemies of type {pair.Key.name}");
        }
    }

    public float GetTimeUntilNextWave()
    {
        if (LevelManager.main.gameEnded == false)
        {
            if (prepBool)
            {
                return preptime;
            }
            else if (!waveCommencing)
            {
                return intermissionTime;
            }
            else
            {
                return waveTimeLength;
            }
        }
        return -1f;  
    }

    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }

}
