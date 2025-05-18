using System.IO;
using UnityEngine;

public class FileSaveManager : PersistSingleton<FileSaveManager>
{
    public GameDataSave GameData { get; private set; }

    private string GetSaveFilePath(string slotName)
    {
        return Path.Combine(Application.persistentDataPath, slotName + ".json");
    }
    public void CreateNewGame(string slotName = "slot1")
    {
        GameData = new GameDataSave();
        SaveGame(slotName);
    }
    public void SaveGame(string slotName = "slot1")
    {
        string filePath = GetSaveFilePath(slotName);

        if (GameManager.Instance.Player.TryGetComponent<PlayerStats>(out var playerStats))
        {
            GameData.player = playerStats.GetSaveData();
        }
        else
        {
            Debug.LogError("SaveGame: PlayerStats component not found!");
            return;
        }

        if (InventoryManager.Instance.Inventory.Count > 0)
        {
            GameData.player.items = InventoryManager.Instance.SaveInventory();
        }

        string json = JsonUtility.ToJson(GameData);
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
                playerStats.SetSavedData(GameData.player);
            }
            else
            {
                Debug.LogError("LoadGame: PlayerStats component not found!");
            }
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.LoadInventory(GameData.player.items);
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