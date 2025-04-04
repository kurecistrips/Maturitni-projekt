using UnityEngine;

public class FarmTower : MonoBehaviour
{
    [SerializeField] private GameObject farmUI;
    
    private bool placedDuringWave = false;
    private bool placedDuringPrep = false;
    private bool giveMoney = false;

    [SerializeField] private Tower tower;

    private void Start()
    {
        if (WaveManager.main.waveCommencing == true)
        {
            placedDuringWave = true;
        }

    }

    private void Update()
    {
        if (WaveManager.main.waveCommencing != false)
        {
            if (giveMoney != true && placedDuringWave != true || placedDuringPrep == true)
            {
                LevelManager.main.IncreaseCurrency(tower.moneyPerWave);
                giveMoney = true;
            }
        }
        else
        {
            giveMoney = false;
            placedDuringWave = false;
        }
    }     
}
