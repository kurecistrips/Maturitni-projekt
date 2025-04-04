using UnityEngine;
using UnityEngine.UI;

public class SkipWaveScript : MonoBehaviour
{
    public GameObject skipPopUp;
    public Button voteYes;
    public Button voteNo;

    public bool vetoed = false;

    public static SkipWaveScript main;

    private void Awake()
    {
        main = this;
    }
    private void Start()
    {
        skipPopUp.SetActive(false);
    }

    public void OnCLickYes()
    {
        WaveManager.main.SkipWave();
    }
    public void OnClickVeto()
    {
        vetoed = true;
        skipPopUp.SetActive(false);
    }


}
