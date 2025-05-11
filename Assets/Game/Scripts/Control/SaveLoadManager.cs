using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }
    public GameDataSave GameData { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string GetSaveFilePath(string slotName)
    {
        return Path.Combine(Application.persistentDataPath, slotName + ".json");
    }
    public void SaveGame(string slotName = "slot1")
    {
        string filePath = GetSaveFilePath(slotName);
        GameDataSave saveData = new();

        if (GameManager.Instance.Player.TryGetComponent<PlayerStats>(out var playerStats))
        {
            saveData.playerStatsDataSave = playerStats.GetSaveData();
        }
        else
        {
            Debug.LogError("SaveGame: PlayerStats component not found!");
            return;
        }


        string json = JsonUtility.ToJson(saveData);
        Debug.Log(json);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, json);
            Debug.Log("Game save into file: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SaveGame: Error saving game data: {e.Message}");
        }
    }

    public void LoadGame(string slotName = "slot1")
    {
        string filePath = GetSaveFilePath(slotName);
        if (!File.Exists(filePath))
        {
            Debug.LogError("LoadGame: File not found: " + filePath);
            return;
        }

        try
        {
            string json = File.ReadAllText(filePath);

            GameData = JsonUtility.FromJson<GameDataSave>(json);

            PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.Stats.SetLoadedModifiers(GameData.playerStatsDataSave.activePlayerModifiers);
                playerStats.Health.SetMaxHealth(GameData.playerStatsDataSave.HP);
            }
            else
            {
                Debug.LogError("LoadGame: PlayerStats component not found!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LoadGame: Error loading game data: {e.Message}");
        }
    }
    public bool IsGameSaved(string slotName = "slot1")
    {
        string filePath = GetSaveFilePath(slotName);
        return File.Exists(filePath);
    }

}