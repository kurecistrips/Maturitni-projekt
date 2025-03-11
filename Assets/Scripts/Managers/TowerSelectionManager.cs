using System.Collections;
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
    public TextMeshProUGUI nameText, damageText, rangeText, costText;
    public Image towerImage;
    public Button equipButton;
    public Image equipButtonColor;
    public TextMeshProUGUI equipButtonText;
    public Color greenColor;
    public Color redColor;

    private string savePath;
    private GameObject selectedTower;

    void Awake()
    {
        main = this;
    }

    private void Start()
    {
        savePath = Application.persistentDataPath + "/selectedTowers.json";
        LoadSelection();
        AssingButtons();
    }

    void AssingButtons()
    {       

        for (int i = 0; i < towerButtons.Length && i < availableTowers.Count; i++) 
        {
            int index = i;
            Tower tower = availableTowers[index].GetComponent<Tower>();
            TowerButtons buttons = towerButtons[index];
            Button button = buttons.button;
            //TowerButtons ulockedTwrs = towerButtons[index];
            //bool unlocked = ulockedTwrs.isUnlocked;
            
            if (tower != null)
            {
                button.GetComponent<Image>().sprite = tower.towerImage;
                button.onClick.AddListener(() => SelectTower(availableTowers[index])); 
            }
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

    void UpdateUI(Tower tower)
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
}

[System.Serializable]
public class TowerSelectionWrapper
{
    public List<string> selectedTowers;
    public TowerSelectionWrapper(List<string> towers) {selectedTowers = towers;}
}

[System.Serializable]
public class TowerButtons
{
    public Button button;
    public bool isUnlocked;
}
