using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

public class LoadoutManager : MonoBehaviour
{
    public static LoadoutManager main;
    public TowerButton[] towerButtons;
    public List<GameObject> selectedTowers = new List<GameObject>();

    public Sprite emptySlotSprite;
    private string emptySlotText = "";
    private GameObject towerPreview;
    private Tower selectedTower;
    private bool isPlacingTower = false;
    private int needed = 0;

    private string savePath;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        savePath = Application.persistentDataPath + "/selectedTowers.json";
        LoadSeletedTowers();
    }
    private void Update()
    {
        if (isPlacingTower && towerPreview != null)
        {
            FollowMouse();
            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacement();
                
            }
            /*else if (Input.GetKeyDown(KeyCode.R)) i tried but not important
            {
                RotatePlacement();
            }*/  
        }
    }

    private void LoadSeletedTowers()
    {
        if (!File.Exists(savePath)) return;
        
        string json = File.ReadAllText(savePath);
        TowerSelectionWrapper loadedData = JsonUtility.FromJson<TowerSelectionWrapper>(json);
        selectedTowers.Clear();
        
        foreach (var towerName in loadedData.selectedTowers)
        {
            GameObject towerPrefab = TowerSelectionManager.main.availableTowers.Find(t => t.GetComponent<Tower>().TowerName == towerName);
            if (towerPrefab)
            {
                selectedTowers.Add(towerPrefab);
            }
        }
        AssignTowersToButtons();
    }
    private void AssignTowersToButtons()
    {
        for (int i = 0; i < towerButtons.Length; i++)
        {
            if (i < selectedTowers.Count)
            {
                Tower tower = selectedTowers[i].GetComponent<Tower>();
                towerButtons[i].buttonImage.sprite = tower.towerImage;
                towerButtons[i].placementCost.text = $"{tower.costToPlace}$";
                towerButtons[i].button.onClick.RemoveAllListeners();
                towerButtons[i].button.onClick.AddListener(() => StartTowerPlacement(tower));
            }
            else
            {
                towerButtons[i].buttonImage.sprite = emptySlotSprite;
                towerButtons[i].placementCost.text = emptySlotText;
                towerButtons[i].button.onClick.RemoveAllListeners();
            }
        }
    }

    private void StartTowerPlacement(Tower tower)
    {
        if (isPlacingTower)  CancelPlacement();
        
        selectedTower = tower;
        if (selectedTower.costToPlace <= LevelManager.main.currency)
        {
            isPlacingTower = true; 

            towerPreview = Instantiate(selectedTower.gameObject);
        }
        else{
            needed = selectedTower.costToPlace - LevelManager.main.currency;
            Debug.Log($"Not enough money {needed}");
            WarningPopUp.main.PopUpWarning(needed);
            selectedTower = null;
        }
        
    }
    private void FollowMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        towerPreview.transform.position = mousePosition;
    }
    private void CancelPlacement()
    {
        isPlacingTower = false;
        Destroy(towerPreview);
        towerPreview = null;
        selectedTower = null;
    }
    public void AfterPlacement()
    {
        LevelManager.main.SpendCurrency(selectedTower.costToPlace);
        isPlacingTower = false;
        Destroy(towerPreview);
        towerPreview = null;
        selectedTower = null;
    }
    public GameObject GetSelectedTower()
    {
        if (selectedTower != null)
        {
            return selectedTower.gameObject;
        }
        else
        {
            return null;
        }   
    }
}
//třída pro načítání informací pro jednotlivé věže v rychlé liště v levelu
[System.Serializable]
public class TowerButton
{
    public Button button;
    public Image buttonImage;
    public TextMeshProUGUI placementCost;
}


