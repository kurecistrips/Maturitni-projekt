using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    public Transform startPoint;
    public Transform[] path;

    private float totalTime = 0f;
    public float showInGameTimer;
    [SerializeField] private int startingCurrency;
    public int currency;
    [SerializeField] private int maxBaseHealth;
    public int maxHealth;
    public int baseHealth;
    public bool gameEnded = false;
    public bool victory = false;

    private void Awake()
    {
        main = this;
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
        if (gameEnded != false)
        {
            totalTime += 0;
            if (baseHealth > 0)
            {
                victory = true;
            }
        }
        else{
            totalTime += Time.deltaTime;
        }
        if (baseHealth <= 0)
        {
            PlayerDeath();
            gameEnded = true;
        }
        
    }

    public void IncreaseCurrency(int amount)
    {
        currency += amount;
    }
    public void SpendCurrency(int amount)
    {
        currency -= amount;
    }

    private void PlayerDeath()
    {
        WaveManagerTest.main.ScoreScreen();
        victory = false;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
