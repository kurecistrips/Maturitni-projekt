using UnityEngine;
using System.IO;

//hlavní skript (pro měnu v hlavní menu)
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager main;
    public int currency;
    private string savePath;

    private void Awake()
    {
        if (main == null) //pokud herní objekt a skript není inicializován
        {
            main = this;   
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/currency.json";
            LoadCurrency();
        }   
    }

    public void AddCurrency(int amount) //funkce na zvýšení měny
    {
        currency += amount;
        SaveCurrency();
    }

    public bool SpendCurrency(int amount) //funkce k utracení měny
    {
        if (currency >= amount)
        {
            currency -= amount;
            SaveCurrency();
            return true;
        }
        WarningPopUp.main.PopUpWarning(amount - currency);
        Debug.Log($"Not enough money");
        return false;
    }
    public void ResetCurrency() //pro vývojáře
    {
        currency = 0;
        SaveCurrency();
    }

    private void SaveCurrency() //funkce na ukládání měny
    {
        File.WriteAllText(savePath, JsonUtility.ToJson(new CurrencyData(currency)));
    }

    private void LoadCurrency() //funkce na načtení měny
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currency = JsonUtility.FromJson<CurrencyData>(json).amount;
        }
    }
}

//třída pro data měny
[System.Serializable]
public class CurrencyData
{
    public int amount; //množství měny
    public CurrencyData(int amount) { this.amount = amount; } //konstruktor pro inicializaci množství měny
}
