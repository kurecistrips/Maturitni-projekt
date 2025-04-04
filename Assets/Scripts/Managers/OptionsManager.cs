using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    public GameObject optionsMenu;

    void Start()
    {
        optionsMenu.gameObject.SetActive(false); //automaticky vypne tabulku
    }
    void Update() //update funkce 
    {
        if (Input.GetKeyDown(KeyCode.Escape) && optionsMenu.gameObject.activeInHierarchy == false) //pokud hráč stiskne ESC tlačítko na klávesnici, tak se zobrazí UI tabluka možností
        {
            optionsMenu.gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && optionsMenu.gameObject.activeInHierarchy == true) //pokud tabulka je zobrazena tak hráč může zas stiknout ESC a tím tabulku zavře
        {
            optionsMenu.gameObject.SetActive(false);
        }
    }
    public void ExitGame()
    {
        SceneManager.LoadScene(0); //hráč se vrátí do hlavního menu
    }
}
