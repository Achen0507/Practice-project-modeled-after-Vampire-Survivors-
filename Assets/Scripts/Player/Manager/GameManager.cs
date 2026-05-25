using Survivor.Data;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public CharacterData SelectedCharacter { get; set; }
    public Dictionary<StatType, float> FinalCharacterStats { get; set; }
    public MapData SelectedMap { get; set; }

    // Ñ¡Ïî¼üÃû
    public const string SFXVolume = "SFXVolume";
    public const string MusicVolume = "MusicVolume";
    public const string Resolution = "Resolution";
    public const string WindowMode = "WindowMode";
    public const string VSync = "VSync";
    public const string Border = "Border";
    public const string PixelFont = "PixelFont";
    public const string ScreenShake = "ScreenShake";
    public const string FlashEffect = "FlashEffect";
    public const string DamageNumber = "DamageNumber";
    public const string Language = "Language";

    private int collectionPoints;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadPoints();
    }

    public void AddCollectionPoints(int amount) {
        collectionPoints += amount;
        SavePoints();
    }

    public int GetCollectionPoints() {
        return collectionPoints;
    }

    private void LoadPoints()
    {
        collectionPoints = PlayerPrefs.GetInt("CollectionPoints", 0);
    }

    private void SavePoints()
    {
        PlayerPrefs.SetInt("CollectionPoints", collectionPoints);
        PlayerPrefs.Save();
    }
}
