using System.IO;
using UnityEngine;
using MemoryPack;
using System;
using Newtonsoft.Json;
using System.Linq.Expressions;

public class FileSaveManager : PersistSingleton<FileSaveManager>
{
    public GameDataSave GameData { get; private set; }

    public event Action OnGameLoaded;
    public event Action OnGameSaved;

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
        string filePath = GetSaveFilePath(slotName, false);
#else
        string filePath = GetSaveFilePath(slotName, true);
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
            string json = JsonConvert.SerializeObject(GameData);
            File.WriteAllText(filePath, json);
            Debug.Log(json);
            Debug.Log("Game saved to file (JSON): " + filePath);
#else
            byte[] bytes = MemoryPackSerializer.Serialize(GameData);
            File.WriteAllBytes(filePath, bytes);
            Debug.Log("Game saved to file (MemoryPack Binary): " + filePath);

#endif
            OnGameSaved?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveGame: Error saving game data: {e.Message}");
        }
    }

    //make async because it's can running really often
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
            string json = File.ReadAllText(filePath);
            GameData = JsonConvert.DeserializeObject<GameDataSave>(json);
            Debug.Log("Game loaded from file (JSON): " + filePath);
#else
            byte[] bytes = File.ReadAllBytes(filePath);
            GameData = MemoryPackSerializer.Deserialize<GameDataSave>(bytes);
            Debug.Log("Game loaded from file (MemoryPack Binary): " + filePath);
#endif

            OnGameLoaded?.Invoke();
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
        catch (Exception e)
        {
            Debug.LogError($"LoadGame: Error loading game data: {e.Message}");
        }
    }
    public bool IsGameSaved(string slotName = "slot1")
    {
#if UNITY_EDITOR
        return File.Exists(GetSaveFilePath(slotName, false));
#else
        return File.Exists(GetSaveFilePath(slotName, true));
#endif
    }

    public bool SaveElement(SavableObject element)
    {
        try
        {
            GameData.saveAbles[element.Id] = element.CaptureState();
            SaveGame();
            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }
    public bool LoadElement(SavableObject element)
    {
        try
        {
            if (GameData != null && GameData.saveAbles.ContainsKey(element.Id))
            {
                element.RestoreState(GameData.saveAbles[element.Id]);
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
        return false;
    }
}