using TMPro;
using UnityEngine;

public class GameOutcome : MonoBehaviour
{
    public GameObject canvas;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI outcomeText;
    private bool isActive = false;
    private int showCoins;
    private float timer;
    private string textOutCome;

    private void Start()
    {
        canvas.SetActive(isActive);

    }

    private void Update()
    {
    	showCoins = WaveManagerTest.main.showReceiveCoins;
        timer = LevelManager.main.showInGameTimer;
        
        if (LevelManager.main.victory == true)
        {
            textOutCome = "Victory";
        }
        else
        {
            textOutCome = "Defeat";
        }
    }

    
    public void Activate()
    {
        isActive = true;
        canvas.SetActive(isActive);
        float minutes = Mathf.FloorToInt(timer / 60);
        float seconds = Mathf.FloorToInt(timer % 60);
        timeText.text = "Time: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        coinText.text = $"Money: {showCoins}";
        outcomeText.text = $"{textOutCome}";

    }
}
