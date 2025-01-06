using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TopUICanvas : MonoBehaviour
{
    public GameObject topUICanvas;
    public TextMeshProUGUI timeBetweenWavesText;
    public TextMeshProUGUI waveCounter;
    public TextMeshProUGUI healthBar;
    
    float time;
    private int hp;

    void Start()
    {
        time = WaveManager.main.timeBetweenWaves;
    }

    private void Update()
    {
        waveCounter.text = $"Wave: {WaveManager.main.waveIndex}"; //this is the only way this thing works
        
        time = WaveManager.main.GetTimeUntilNextWave() + 1;

        if (time <= 0)
        {
            time = WaveManager.main.timeBetweenWaves;
        }
        

        time -= Time.deltaTime;
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        timeBetweenWavesText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
