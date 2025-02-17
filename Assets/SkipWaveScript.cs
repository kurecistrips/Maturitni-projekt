using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipWaveScript : MonoBehaviour
{
    public GameObject skipPopUp;
    public Button voteYes;
    public Button voteNo;

    private bool show = false;

    void Update()
    {
        show = WaveManagerTest.main.showSkipPopUp();
        skipPopUp.SetActive(show);
        
    }
}
