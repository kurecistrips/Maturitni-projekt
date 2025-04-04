using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

//hlavní skript
public class TowerSelectionManager : MonoBehaviour
{
    public static TowerSelectionManager main;
    public TowerButtons[] towerButtons; //pole tlačítek pro věže
    public List<GameObject> availableTowers; //seznam dostupných věží
    public List<GameObject> selectedTowers = new List<GameObject>(); //seznam vybraných věží
    public HotBar[] hotBar; //odkazy na sloty v rychlé liště (aka hotbar)
    public TextMeshProUGUI nameText, damageText, rangeText, costText; //UI texty
    public Image towerImage; //obrázek věžě v UI
    public Button equipButton; //tlačítko pro vybavení věže
    public Button buyButton; //tlačítko pro nákup věže
    public GameObject lockedTowerGO; //UI element pro zamčenou věž
    public TextMeshProUGUI buyButtonText; //UI text element pro nákup věže
    public Image equipButtonColor; //UI element pro změnu barvy
    public TextMeshProUGUI equipButtonText; //UI text element pro tlačítko
    //barvy pro tlačítka
    public Color greenColor; 
    public Color redColor;
    //cesty k uloženým souborům
    private string savePath; 
    private string unlockedSavePath; 
    private GameObject selectedTower; //aktuálně vybraná věž

    public Button resetBought; //testující tlačítka pro vývojáře

    public Sprite emptySlotSprite; //obrázek (sprite) pro prázdný slot

    private void Awake()
    {
        main = this; //nastavení instance main
    }

    private void Start()
    {
        //nastavení cest pro ukládání
        savePath = Application.persistentDataPath + "/selectedTowers.json";
        unlockedSavePath = Application.persistentDataPath + "/unlockedTowers.json";

        LoadSelection();
        LoadTowerUnlocks();
        
        AssignButtons(); //přiřazení funkcí tlačítkům věží
        AssignTowersToHotBar(); //naplnění rychlé lišty věžěmí
        UpdateUIOnStart(); //inicializace UI


        resetBought.onClick.AddListener(ResetTowerUnlocks); //přidání event listeneru pro reset (dev only)
    }

    private void AssignButtons() //funkce pro řiřazení věží k tlačítkům
    {       
        //pro každé tlačítko věžě nastavíme obrázek a klikací akci
        for (int i = 0; i < towerButtons.Length; i++) 
        {
            int index = i;
            Tower tower = availableTowers[index].GetComponent<Tower>();
            TowerButtons buttons = towerButtons[index];
            Button btn = buttons.button;
            
            
            btn.GetComponent<Image>().sprite = tower.towerImage;
            btn.onClick.AddListener(() => SelectTower(availableTowers[index])); //přiřazení nového posluchače událostí, který zavolá metodu
        }
    }

    private void UpdateUIOnStart() //funkce k updatnutí UI elementů při spuštění
    {
        LoadTowerUnlocks(); //načtení otevřených věží

        //aktualizace UI podle odemknutých věží
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
        SelectFirstTower(); //výbraní prní věže (aby se neukazovalo nic)
    }

    private void SelectFirstTower() //funkce k vybrání první věže
    {
        //automatický výběr dostupné věže
        for (int i = 0; i < availableTowers.Count; i++) //nevím proč i++ má problém (it just works)
        {
            TowerButtons towerButton = towerButtons[i]; 

            SelectTower(availableTowers[i]);

            towerButton.button.Select();
            return;
        }
    }

    public void SelectTower(GameObject towerObj) //funkce na aktualizaci UI
    {
        //nastavení aktuálně vybrané věže a akualizaci UI
        Tower tower = towerObj.GetComponent<Tower>();
        if (tower != null)
        {
            selectedTower = towerObj;
            UpdateUI(tower);   
        }
    }

    private void UpdateUI(Tower tower) //funkce na updatnutí UI elementů
    {
        //aktualizace informací věže v UI
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

        //nastavení viditelnosti tlačítka podle dostupnosti věže
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
            
            buyButton.onClick.RemoveAllListeners(); //odstranění předešlých posluchačů událostí na tlačítku (aby se neopakovaly stejné instance stejné funkce)
            buyButton.onClick.AddListener(() => BuyTower(towerButton, tower)); //přiřazení nového posluchače událostí, který zavolá metodu
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
            equipButton.onClick.RemoveAllListeners(); //odstranění předešlých posluchačů událostí na tlačítku (aby se neopakovaly stejné instance stejné funkce)
            equipButton.onClick.AddListener(() => ToggleEquipTower(selectedTower)); //přiřazení nového posluchače událostí, který zavolá metodu
        }
    }

    public void ToggleEquipTower(GameObject towerObj) //funkce na přiřazení věže do loadoutu (vybavení věží)
    {
        //přepínání mezi vybavením a odebráním věže
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
        SaveSelection(); //uloží výběr
        AssignTowersToHotBar(); //přiřadí věže do rychlé lišty
    }

    public void SaveSelection() //funkce na ukládání vybráných věží
    {
        //uložnení do souboru
        List<string> towerNames = new List<string>();
        foreach (var tower in selectedTowers)
        {
            towerNames.Add(tower.GetComponent<Tower>().TowerName);
        }
        string json = JsonUtility.ToJson(new TowerSelectionWrapper(towerNames));
        File.WriteAllText(savePath, json);
    }

    public void LoadSelection() //funkce na načtení vybranných věží 
    {
        //načtení ze souboru
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

    private void ResetSelection() //pro vývojáře
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

    public void BuyTower(TowerButtons button, Tower tower) //funkce pro nákup věží a otevření
    {

        if (CurrencyManager.main.SpendCurrency(button.price))
        {
            button.isUnlocked = true;
            SaveTowerUnlocks();
                
            UpdateUI(tower);
        }
    }

    public void SaveTowerUnlocks() //funkce na ukládání otevřených věží
    {
        List<bool> unlockedStates = new List<bool>();
        foreach (var button in towerButtons)
        {
            unlockedStates.Add(button.isUnlocked);
        }
        string json = JsonUtility.ToJson(new TowerUnlockWrapper(unlockedStates));
        File.WriteAllText(unlockedSavePath, json);
    }

    public void LoadTowerUnlocks() //funkce na načtení otevřených věží
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

    public void ResetTowerUnlocks() //pro vývojáře
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
    private void UpdateUIAfterReset() //pro vývojáře
    {
        foreach (TowerButtons button in towerButtons)
        {
            if (!button.isUnlocked)
            {
                lockedTowerGO.SetActive(true);
            }
        }
    }

    private void AssignTowersToHotBar() //funkce na přiřazení věží do rychlé lišty 
    {
        //přiřazení do lišty
        for (int i = 0; i < 6; i++)
        {
            if (i < selectedTowers.Count)
            {
                Tower tower = selectedTowers[i].GetComponent<Tower>();
                hotBar[i].hotBarImage.sprite = tower.towerImage;
                int index = i;
                hotBar[i].hotBarButton.onClick.RemoveAllListeners(); //odstranění předešlých posluchačů událostí na tlačítku (aby se neopakovaly stejné instance stejné funkce)
                hotBar[i].hotBarButton.onClick.AddListener(() => SelectTower(selectedTowers[index])); //přiřazení nového posluchače událostí, který zavolá metodu

            }
            else //jesli slot nemá přiřazenou věž tak zobrazí obrázek pro prázdný slot
            {
                hotBar[i].hotBarImage.sprite = emptySlotSprite;
                hotBar[i].hotBarButton.onClick.RemoveAllListeners(); //odstranění předešlých posluchačů událostí na tlačítku (aby se neopakovaly stejné instance stejné funkce)
            }
        }
    }


}

//třída pro ukládání seznamu vybraných věží do souboru
[System.Serializable]
public class TowerSelectionWrapper
{
    public List<string> selectedTowers; //seznam vybraných věží podle názvu
    public TowerSelectionWrapper(List<string> towers) {selectedTowers = towers;} //konstruktor pro inicializaci seznamu věži
}

//třída pro uchovávání seznamu odemčených věží
[System.Serializable]
public class TowerUnlockWrapper
{
    public List<bool> unlockedTowers; //seznam stavů odemčení věží
    public TowerUnlockWrapper(List<bool> unlocked) { unlockedTowers = unlocked; } //konstruktor pro inicializaci seznamů odemčených věží
}

//třída reprezentující tlačítko ve výběru věží v UI
[System.Serializable]
public class TowerButtons
{
    public Button button; //tlačítko pro výběr věže
    public GameObject towerPrefab; //prefab věžě přizený k tlačítku
    public bool isUnlocked; //označuje, zda je věž odemčená
    public int price; //cena věže
    public GameObject lockedTowerButtonGO; //ui objekt indikující zamčenou věž
    public TextMeshProUGUI towerPrice; //UI text zobrazující cenu věže
}

//třída reprezentující sloty v rychlé liště (hotbar)
[System.Serializable]
public class HotBar
{
    public Button hotBarButton; //slot tlačítko
    public Image hotBarImage; //obrázek pro slot
}
