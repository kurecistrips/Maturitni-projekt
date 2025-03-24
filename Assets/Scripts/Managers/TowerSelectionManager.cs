using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class TowerSelectionManager : MonoBehaviour
{
    public static TowerSelectionManager main;
    public TowerButtons[] towerButtons;
    public List<GameObject> availableTowers;
    public List<GameObject> selectedTowers = new List<GameObject>();
    public HotBar[] hotBar;
    public TextMeshProUGUI nameText, damageText, rangeText, costText;
    public Image towerImage;
    public Button equipButton;
    public Button buyButton;
    public GameObject lockedTowerGO;
    public TextMeshProUGUI buyButtonText;
    public Image equipButtonColor;
    public TextMeshProUGUI equipButtonText;
    public Color greenColor;
    public Color redColor;

    private string savePath;
    private string unlockedSavePath;
    private GameObject selectedTower;

    public Button resetBought;

    public Sprite emptySlotSprite;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        savePath = Application.persistentDataPath + "/selectedTowers.json";
        unlockedSavePath = Application.persistentDataPath + "/unlockedTowers.json";

        LoadSelection();
        LoadTowerUnlocks();
        
        AssingButtons();
        AssignTowersToHotBar();
        UpdateUIOnStart();


        resetBought.onClick.AddListener(ResetTowerUnlocks);
    }

    private void AssingButtons()
    {       

        for (int i = 0; i < towerButtons.Length && i < availableTowers.Count; i++) 
        {
            int index = i;
            Tower tower = availableTowers[index].GetComponent<Tower>();
            TowerButtons buttons = towerButtons[index];
            Button btn = buttons.button;
            
            
            if (tower != null)
            {
                btn.GetComponent<Image>().sprite = tower.towerImage;
                btn.onClick.AddListener(() => SelectTower(availableTowers[index])); 
            }
        }
    }

    private void UpdateUIOnStart()
    {
        LoadTowerUnlocks();

        foreach (var button in towerButtons)
        {
            if (button.isUnlocked == false)
            {
                if (button.price == 0)
                {
                    button.towerPrice.text = $"FREE";
                }
                else
                {
                    button.towerPrice.text = $"{button.price}$";
                }
                    
                button.lockedTowerButtonGO.SetActive(true);
            }
            else
            {
                button.lockedTowerButtonGO.SetActive(false);
            }
        }
        SelectFirstTower();
    }

    private void SelectFirstTower()
    {
        for (int i = 0; i < availableTowers.Count; i++)
        {
            TowerButtons towerButton = towerButtons[i];

            SelectTower(availableTowers[i]);

            towerButton.button.Select();
            return;
        }
    }

    public void SelectTower(GameObject towerObj)
    {
        Tower tower = towerObj.GetComponent<Tower>();
        if (tower != null)
        {
            selectedTower = towerObj;
            UpdateUI(tower);   
        }
    }

    private void UpdateUI(Tower tower)
    {
        
        nameText.text = tower.name;
        damageText.text = $"Damage: {tower.damage}";
        damageText.color = Color.red;
        rangeText.text = $"Range: {tower.attackRange}";
        rangeText.color = Color.blue;
        costText.text = $"Cost: {tower.costToPlace}$";
        costText.color = Color.green;
        towerImage.sprite = tower.towerImage;
        towerImage.color = new Color (1,1,1,1);

        TowerButtons towerButton = FindTowerButton(tower);

        if (towerButton != null && !towerButton.isUnlocked)
        {
            towerButton.lockedTowerButtonGO.SetActive(true);
            lockedTowerGO.SetActive(true);
            if (towerButton.price == 0)
            {
                buyButtonText.text = $"Buy for: FREE";
                towerButton.towerPrice.text = "FREE";
            }
            else
            {
                buyButtonText.text = $"Buy for: {towerButton.price}$";
                towerButton.towerPrice.text = $"{towerButton.price}$";
            }
            
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => BuyTower(towerButton, tower));
        }
        else
        {
            towerButton.lockedTowerButtonGO.SetActive(false);
            lockedTowerGO.SetActive(false);
            equipButton.gameObject.SetActive(true);
            
            if (selectedTowers.Contains(selectedTower))
            {
                equipButtonText.text = "Unequip";
                equipButtonColor.color = redColor;
            }
            else
            {
                equipButtonText.text = "Equip";
                equipButtonColor.color = greenColor;
            }
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(() => ToggleEquipTower(selectedTower));
        }
    }

    public void ToggleEquipTower(GameObject towerObj)
    {
        if (selectedTowers.Contains(towerObj))
        {
            selectedTowers.Remove(towerObj);
            equipButtonText.text = "Equip";
            equipButtonColor.color = greenColor;
        }
        else if (selectedTowers.Count < 6)
        {
            selectedTowers.Add(towerObj);
            equipButtonText.text = "Unequip";
            equipButtonColor.color = redColor;
        }
        SaveSelection();
        AssignTowersToHotBar();
    }

    public void SaveSelection()
    {
        List<string> towerNames = new List<string>();
        foreach (var tower in selectedTowers)
        {
            towerNames.Add(tower.GetComponent<Tower>().TowerName);
        }
        string json = JsonUtility.ToJson(new TowerSelectionWrapper(towerNames));
        File.WriteAllText(savePath, json);
    }

    public void LoadSelection()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            TowerSelectionWrapper loadedData = JsonUtility.FromJson<TowerSelectionWrapper>(json);
            
            selectedTowers.Clear();
            foreach (var towerName in loadedData.selectedTowers)
            {
                GameObject towerPrefab = availableTowers.Find(t => t.GetComponent<Tower>().TowerName == towerName);
                if (towerPrefab)
                {
                    selectedTowers.Add(towerPrefab);
                }
            }
        }
    }

    private void ResetSelection()
    {
        selectedTowers.Clear();
        SaveSelection();
    }
    
    private TowerButtons FindTowerButton(Tower tower)
    {
        foreach (TowerButtons btn in towerButtons)
        {
            if (btn.towerPrefab == tower.gameObject)
            {
                return btn;
            }
        }
        return null;
    }

    public void BuyTower(TowerButtons button, Tower tower)
    {

        if (CurrencyManager.main.SpendCurrency(button.price))
        {
            button.isUnlocked = true;
            SaveTowerUnlocks();
                
            UpdateUI(tower);
        }
    }

    public void SaveTowerUnlocks()
    {
        List<bool> unlockedStates = new List<bool>();
        foreach (var button in towerButtons)
        {
            unlockedStates.Add(button.isUnlocked);
        }
        string json = JsonUtility.ToJson(new TowerUnlockWrapper(unlockedStates));
        File.WriteAllText(unlockedSavePath, json);
    }

    public void LoadTowerUnlocks()
    {
        if (File.Exists(unlockedSavePath))
        {
            string json = File.ReadAllText(unlockedSavePath);
            TowerUnlockWrapper loadedData = JsonUtility.FromJson<TowerUnlockWrapper>(json);

            for (int i = 0; i < towerButtons.Length && i < loadedData.unlockedTowers.Count; i++)
            {
                towerButtons[i].isUnlocked = loadedData.unlockedTowers[i];
            }
        }
    }

    public void ResetTowerUnlocks()
    {
    
        foreach (TowerButtons button in towerButtons)
        {
            button.isUnlocked = false;
            UpdateUIAfterReset();
        }
        SaveTowerUnlocks();
        ResetSelection();
        CurrencyManager.main.ResetCurrency();
        
    } 
    private void UpdateUIAfterReset()
    {
        foreach (TowerButtons button in towerButtons)
        {
            if (!button.isUnlocked)
            {
                lockedTowerGO.SetActive(true);
            }
        }
    }

    private void AssignTowersToHotBar()
    {
        for (int i = 0; i < 6; i++)
        {
            if (i < selectedTowers.Count)
            {
                Tower tower = selectedTowers[i].GetComponent<Tower>();
                hotBar[i].hotBarImage.sprite = tower.towerImage;
                int index = i;
                hotBar[i].hotBarButton.onClick.RemoveAllListeners();
                hotBar[i].hotBarButton.onClick.AddListener(() => SelectTower(selectedTowers[index]));

            }
            else
            {
                hotBar[i].hotBarImage.sprite = emptySlotSprite;
                hotBar[i].hotBarButton.onClick.RemoveAllListeners();
            }
        }
    }


}

[System.Serializable]
public class TowerSelectionWrapper
{
    public List<string> selectedTowers;
    public TowerSelectionWrapper(List<string> towers) {selectedTowers = towers;}
}
[System.Serializable]
public class TowerUnlockWrapper
{
    public List<bool> unlockedTowers;
    public TowerUnlockWrapper(List<bool> unlocked) { unlockedTowers = unlocked; }
}

[System.Serializable]
public class TowerButtons
{
    public Button button;
    public GameObject towerPrefab;
    public bool isUnlocked;
    public int price;
    public GameObject lockedTowerButtonGO;
    public TextMeshProUGUI towerPrice;
}
[System.Serializable]
public class HotBar
{
    public Button hotBarButton;
    public Image hotBarImage;
}
