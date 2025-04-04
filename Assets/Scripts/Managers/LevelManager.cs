using UnityEngine;
using UnityEngine.SceneManagement;

//hlavní skript
public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    public Transform[] path; //seznam bodů cesty

    private float totalTime = 0f; //jak dlouho hráč je ve hře 
    public float showInGameTimer; //zobrazení času pro UI elementy
    [SerializeField] private int startingCurrency; //měna, kterou hráč dostane na začátku
    public int currency; //měna, za kterou hráč může koupit věže
    [SerializeField] private int maxBaseHealth; //maximalní počet životů hráče
    public int maxHealth; //pro zobrazování v UI elementech
    public int baseHealth; //základní počet životů hráče
    public bool gameEnded = false; //určuje, zda hra skončila
    public bool victory = false; //určuje, zda hráč vyhrál 

    private void Awake()
    {
        main = this; //nastavení instance main
    }

    public void Start()
    {
        maxHealth = maxBaseHealth; 
        baseHealth = maxBaseHealth;
        currency = startingCurrency;
    }

    public void Update()
    {
        showInGameTimer = totalTime;
        if (gameEnded != false) //pokud hra skončila
        {
            totalTime += 0;
            if (baseHealth > 0)  //a hráč žije
            {
                victory = true;
                WaveManager.main.WinBonus();
            }
        }
        else 
        {
            totalTime += Time.deltaTime;
        }
        if (baseHealth <= 0) //pokud hráč nepřežije
        {
            PlayerDeath();
            gameEnded = true;
        }
        
    }

    public void IncreaseCurrency(int amount) //funkce na zvýšení peněz
    {
        currency += amount;
    }
    public void SpendCurrency(int amount) //funkce na utracení peněz
    {
        currency -= amount;
    }

    private void PlayerDeath() //funkce pokud hráč nepřežije
    {
        WaveManager.main.ScoreScreen();
    }

    public void ReturnToMenu() //funkce na návrat do hlavního menu
    {
        SceneManager.LoadScene("Main Menu");
    }
}
