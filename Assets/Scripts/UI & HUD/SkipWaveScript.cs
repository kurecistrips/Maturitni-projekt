using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipWaveScript : MonoBehaviour
{
    public GameObject skipPopUp;
    public Button voteYes;
    public Button voteNo;

    public bool veoted = false;

    public static SkipWaveScript main;

    void Awake()
    {
        main = this;
    }
    void Start()
    {
        skipPopUp.SetActive(false);
    }

    public void OnCLickYes()
    {
        WaveManagerTest.main.SkipWave();
    }
    public void OnClickVeto()
    {
        veoted = true;
        skipPopUp.SetActive(false);
    }


}
