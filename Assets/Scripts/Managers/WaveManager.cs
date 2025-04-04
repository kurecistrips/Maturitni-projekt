using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//třída obsahující informace o spawnování nepřátel
[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab; 
    public int count = 1; //počet neprátel ke spawnutí
    public bool hidden = false; //zda je nepřítel neviditelný (true: je|false: není)
    public float timeBetweenSpawns = 0.5f; //čas mezi spawnování jednotlivých nepřátel
    public float spawnAfterStartTime = 0f; //kdy se má dávka jednotek spawnout po startu vlny
}

//třída reprezentující vlnu nepřátel
[System.Serializable]
public class Wave
{
    public List<EnemySpawnInfo> enemies; //seznam nepřátel ve vlně
    public float timeBetweenWaves; //čas mezi vlnami
    public int currencyPerWave; //měna získaná za dokončení vlny
}

//hlavní skript
public class WaveManager : MonoBehaviour
{
    public static WaveManager main; 
    public List<Wave> waves; //seznam vln nepřátel
    public Transform spawnPoint; //startující místo, kde se spawnují nepřátelé
    public GameOutcome gameOutcome; //odkaz na výsledek hry

    public static UnityEvent onEnemyDestroy = new UnityEvent(); //událost volána při zníčení nepřítele

    private int currentWaveIndex = 0; //index akutální vlny
    public int waveIndex = 1; //číslo vlny (využívané v UI elementech)
    public float waveStartTimeDebug; //čas spuštění aktuální vlny
    private float intermissionTime = 5f; //čas mezi vlnymi
    private float timeSinceGameStart; //čas od začátku hry
    private float preptime = 20f; //čas na přípravu před první vlnou
    private bool prepBool = true; //označení, zda je fáze přípravy běží
    private float waveTimeLength; //délka trvání vlny

    private int enemiesLeftToSpawn; //počet nepřátel zbývajících ke spawnutí
    private int enemiesAlive; //počet aktuálně živých nepřátel
    public bool waveCommencing = false; //označení, zda je vlna běží
    [SerializeField] private int getCoins = 100; //výchoží počet mincí, které hráč získá za dokončení levelu 
    private float victoryBonus = 0.5f; //bonus za vítěztví (nefunguje)
    public int showReceiveCoins; //zobrazené získané mince (pro UI elementy)
    private float recievedCoins; //skutečné přijaté mince
    private float maxAmountOfWaves; //maximální počet vln

    private bool currencyGiven = false; //zda byly přiděleny mince za vlnu
    private bool skipBool = false; //zda byla vlna přeskočen

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed); //přidá posluchače pro událost zníčení nepřítele
        main = this; //nastavení instance main
    }

    private void Start() 
    {
        waveStartTimeDebug = Time.time; //instance času spuštění vlny
        maxAmountOfWaves = waves.Count; //nastavení maximalního počtu vln
    }

    private void Update()
    {
        timeSinceGameStart += Time.deltaTime; //aktualizace času od začátku vlny
        

        if (prepBool)
        {
            PrepTime(); //fáze přípravy
        }        
        else
        {
            preptime += 0;
            
            if (currentWaveIndex < waves.Count)
            {
                if (waveCommencing == true && currentWaveIndex == 0)
                {
                    waveTimeLength = waves[currentWaveIndex].timeBetweenWaves;
                    SpawnWave(); //spuštění první vlny
                    waveIndex++;
                    
                }
                else if (waveTimeLength <= 0 || enemiesAlive == 0 && enemiesLeftToSpawn <= 0 && skipBool == false && LevelManager.main.gameEnded == false)
                {
                    
                    Intermission(); //přestávka mezi vlnami
                    
                }
                else if (skipBool == true) 
                {
                    Intermission(); //přeskočení vlny
                }
                
            }
            else if(enemiesAlive <= 0 && enemiesLeftToSpawn == 0)
            {
                ScoreScreen(); //zobrazení výsledků hry
            }
            waveTimeLength -= Time.deltaTime;
            showSkipPopUp(); //kontrola zobrazení možnosti přeskočení vlny
            
        }
    }

    public void showSkipPopUp() //funkce určující kdy se má UI element pro přeskočení vlny zobrazit
    {
        
        if (waveTimeLength <= waves[waveIndex-2].timeBetweenWaves * 0.90 && waveCommencing == true && prepBool == false && currentWaveIndex <= waves.Count-1 && SkipWaveScript.main.vetoed == false) //hnus, já vím
        {
            SkipWaveScript.main.skipPopUp.SetActive(true);
            
        }
        else {
            SkipWaveScript.main.skipPopUp.SetActive(false);
        }
    }

    public void SkipWave() //fukce na přeskočení vlny
    {
        skipBool = true; 
        Intermission();
        
    }

    private void PrepTime() //funkce na přípravný čas
    {
        preptime -= Time.deltaTime;
        if (preptime <= 0)
        {
            prepBool = false;
            waveCommencing = true;
        }
    }
    
    private void Intermission() //funkce na mezičas
    {
        //označení nepřáel, kteří přežili z předchozí vlny
        EnemyMovement[] activeEnemies = FindObjectsOfType<EnemyMovement>(); 
        foreach (EnemyMovement enemy in activeEnemies)
        {
            enemy.fromLastWave = true;
        }

        waveCommencing = false;

        if (!currencyGiven) //za odměna byla přidělena (musí tam být kontrola, jelikož funkce je v Update funkci, která se updatuje s každým obrázkem)
        {
            LevelManager.main.IncreaseCurrency(waves[waveIndex-2].currencyPerWave);
            currencyGiven = true;
            BonusMoneyPopUp.main.MoneyPerWave(waves[waveIndex-2].currencyPerWave);
        }
        
        enemiesAlive = 0;
        intermissionTime -= Time.deltaTime;
        if (intermissionTime <= 0) //konec mezičasu
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

    private void SpawnWave() //funkce na spawnování vlny
    {
        
        if (currentWaveIndex >= waves.Count) return;

        Debug.Log($"wave: {waveIndex} is spawning");

        Wave currentWave = waves[currentWaveIndex];

        enemiesLeftToSpawn = CalculateTotalEnemiesInWave(currentWave);

        Dictionary<GameObject, int> enemyCounts = CountEnemiesByType(currentWave);
        LogEnemyCounts(currentWaveIndex + 1, enemyCounts);

        waveStartTimeDebug = Time.time;

        StartCoroutine(SpawnEnemiesInWave(currentWave)); //spawnování nepřátel

        currentWaveIndex++;
        
        waveCommencing = true;
    }

    public void ScoreScreen() //funkce na zobrazení výsledků konce levelu
    {
        gameOutcome.Activate();
        waveCommencing = false;
        recievedCoins = getCoins * (currentWaveIndex / maxAmountOfWaves);
        showReceiveCoins = (int)recievedCoins;
        
        /*if (!rewardGiven)
        {
            CurrencyManager.main.AddCurrency((int)recievedCoins);
            rewardGiven = true;
        }*/
        LevelManager.main.gameEnded = true;  
    }

    public void WinBonus() //nefunguje
    {
        Debug.Log("WinBonus called. Before bonus: " + recievedCoins);
        recievedCoins += recievedCoins*victoryBonus;
        Debug.Log("After bonus: " + recievedCoins);
    }

    private IEnumerator SpawnEnemiesInWave(Wave wave) //funcke na spawnování jednotek s použitím vestavené unity delay funkce IEnumerator
    {
        for (int i = 0; i < wave.enemies.Count; i++)
        {
            EnemySpawnInfo enemyInfo = wave.enemies[i];

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

    private int CalculateTotalEnemiesInWave(Wave wave) //funkce na napočítání nepřátel ve vlně
    {
        int totalEnemies = 0;
        enemiesAlive = 0;

        foreach (EnemySpawnInfo enemyInfo in wave.enemies)
        {
            totalEnemies += enemyInfo.count; //sečte všechny nepřátelské jednotky, které se mají objevit v momentální vlně
        }
        return totalEnemies;
    }

    private Dictionary<GameObject, int> CountEnemiesByType(Wave wave) //funkce na počítání typů jednotek
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

    private void LogEnemyCounts(int waveNum, Dictionary<GameObject, int> enemyCounts) //debug funkce
    {
        Debug.Log($"Wave {waveNum} enemy counts:");
        foreach (KeyValuePair<GameObject, int> pair in enemyCounts)
        {
            Debug.Log($"{pair.Value} enemies of type {pair.Key.name}");
        }
    }

    public float GetTimeUntilNextWave() //funkce k vizualizaci času v TopUICanvas
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

    private void EnemyDestroyed() //funkce na odečítání nepřátel
    {
        enemiesAlive--;
    }

}
