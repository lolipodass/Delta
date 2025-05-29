using System.IO;
using UnityEngine;
using MemoryPack;

public class FileSaveManager : PersistSingleton<FileSaveManager>
{
    public GameDataSave GameData { get; private set; }

    private string GetSaveFilePath(string slotName, bool isBinary = false)
    {
        string extension = isBinary ? ".bin" : ".json";
        return Path.Combine(Application.persistentDataPath, slotName + extension);
    }
    public void CreateNewGame(string slotName = "slot1")
    {
        GameData = new GameDataSave();
        SaveGame(slotName);
    }
    public void SaveGame(string slotName = "slot1")
    {
#if UNITY_EDITOR
        string filePath = GetSaveFilePath(slotName, true);
#else
        string filePath = GetSaveFilePath(slotName, false);
#endif

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

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
#if UNITY_EDITOR
            byte[] bytes = MemoryPackSerializer.Serialize(GameData);
            File.WriteAllBytes(filePath, bytes);
            Debug.Log("Game saved to file (MemoryPack Binary): " + filePath);
#else
            string json = JsonUtility.ToJson(GameData);
            File.WriteAllText(filePath, json);
            Debug.Log(json);
            Debug.Log("Game saved to file (JSON): " + filePath);
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SaveGame: Error saving game data: {e.Message}");
        }
    }

    public void LoadGame(string slotName = "slot1")
    {
#if UNITY_EDITOR
        string filePath = GetSaveFilePath(slotName, false);
#else
        string filePath = GetSaveFilePath(slotName, true); 
#endif
        if (!File.Exists(filePath))
        {
            Debug.LogError("LoadGame: File not found: " + filePath);
            return;
        }

        try
        {
#if UNITY_EDITOR
            byte[] bytes = File.ReadAllBytes(filePath);
            GameData = MemoryPackSerializer.Deserialize<GameDataSave>(bytes);
            Debug.Log("Game loaded from file (MemoryPack Binary): " + filePath);
#else
            string json = File.ReadAllText(filePath);
            GameData = JsonUtility.FromJson<GameDataSave>(json);
            Debug.Log("Game loaded from file (JSON): " + filePath);
#endif

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
#if UNITY_EDITOR
        return File.Exists(GetSaveFilePath(slotName, true));
#else
        return File.Exists(GetSaveFilePath(slotName, false));
#endif
    }

}