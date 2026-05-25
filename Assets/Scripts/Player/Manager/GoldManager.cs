using Survivor.MainMenu;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    private int totalGold;
    private int totalKills;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadData();
    }

    public int GetGold() => totalGold;
    public int GetTotalKills() => totalKills;

    public void AddGold(int amount) {
        totalGold += amount;
        SaveData();

        MainMenu mainMenu = FindObjectOfType<MainMenu>();
        if (mainMenu != null) mainMenu.UpdateGoldUI();
    }
    public void AddKill() 
    {
        totalKills++;
        SaveData();
    }

    public void SpendGold(int amount) {
        totalGold -= amount;
        SaveData();

        MainMenu mainMenu = FindObjectOfType<MainMenu>();
        if (mainMenu != null) mainMenu.UpdateGoldUI();
    }

    private void LoadData()
    {
        totalGold = PlayerPrefs.GetInt("TotalGold", 0);
        totalKills = PlayerPrefs.GetInt("TotalKills", 0);  
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("TotalGold", totalGold);
        PlayerPrefs.SetInt("TotalKills", totalKills);  
        PlayerPrefs.Save();
    }
}
