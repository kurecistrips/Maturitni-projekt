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
    
    float time;

    void Start()
    {
        //time = WaveManager.main.waves[waveIndex].timeBetweenWaves;
    }

    private void Update()
    {
        waveCounter.text = $"Wave: {WaveManagerTest.main.waveIndex}"; // This is the only way this thing works
        
        time = WaveManagerTest.main.GetTimeUntilNextWave()+1;

        
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        timeBetweenWavesText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        
    }
}
