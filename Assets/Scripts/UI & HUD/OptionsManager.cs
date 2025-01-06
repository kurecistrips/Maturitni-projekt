using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public GameObject optionsMenu;

    void Start()
    {
        optionsMenu.gameObject.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && optionsMenu.gameObject.activeInHierarchy == false)
        {
            optionsMenu.gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && optionsMenu.gameObject.activeInHierarchy == true)
        {
            optionsMenu.gameObject.SetActive(false);
        }
    }
    public void ExitGame()
    {
        SceneManager.LoadScene(0);
    }
}
