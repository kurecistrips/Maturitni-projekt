using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{

    public GameObject mainMenuPanel;
    public GameObject levelSelectionPanel;
    public GameObject towersSelection;
    
    

    private void Awake()
    {  


    }

    public void OnCLickPlay()
    {
        mainMenuPanel.SetActive(false);
        levelSelectionPanel.SetActive(true);
    }

    public void OnClickExit()
    {
        mainMenuPanel.SetActive(true);
        levelSelectionPanel.SetActive(false);
        towersSelection.SetActive(false);
    }

    public void OpenLevel(int idLvl)
    {
            string level = "Level " + idLvl;
            SceneManager.LoadScene(level);

    }
    public void OnClickTowers()
    {
        mainMenuPanel.SetActive(false);
        towersSelection.SetActive(true);

    }
        
}
