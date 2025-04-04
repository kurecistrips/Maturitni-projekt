using TMPro;
using UnityEngine;

public class TopUICanvas : MonoBehaviour
{
    public GameObject topUICanvas;
    public TextMeshProUGUI timeBetweenWavesText;
    public TextMeshProUGUI waveCounter;
    
    private float time;

    private void Start()
    {
        //time = WaveManager.main.waves[waveIndex].timeBetweenWaves;
    }

    private void Update()
    {
        waveCounter.text = $"Wave: {WaveManager.main.waveIndex-1}"; // This is the only way this thing works
        
        time = WaveManager.main.GetTimeUntilNextWave()+1;

        if (time > 3600)
        {
            timeBetweenWavesText.text = "∞";
        }
        else
        {
            float minutes = Mathf.FloorToInt(time / 60);
            float seconds = Mathf.FloorToInt(time % 60);

            timeBetweenWavesText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        

        
    }
}
