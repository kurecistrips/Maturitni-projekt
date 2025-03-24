using UnityEngine;
using System.IO;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager main;
    public int currency;
    private string savePath;

    private void Awake()
    {
        if (main == null)
        {
            main = this;   
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/currency.json";
            LoadCurrency();
        }   
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        SaveCurrency();
    }

    public bool SpendCurrency(int amount)
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
    public void ResetCurrency()
    {
        currency = 0;
        SaveCurrency();
    }

    private void SaveCurrency()
    {
        File.WriteAllText(savePath, JsonUtility.ToJson(new CurrencyData(currency)));
    }

    private void LoadCurrency()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currency = JsonUtility.FromJson<CurrencyData>(json).amount;
        }
    }
}

[System.Serializable]
public class CurrencyData
{
    public int amount;
    public CurrencyData(int amount) { this.amount = amount; }
}
