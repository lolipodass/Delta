using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDataSave
{

    [System.Serializable]
    public class PlayerStatsDataSave
    {
        public List<UpgradeModifier> activePlayerModifiers;
        public int HP;
        public string savePointName;
        public Vector3 SavePointPosition;
    }

    public PlayerStatsDataSave playerStatsDataSave;

    public GameDataSave()
    {
        playerStatsDataSave = new PlayerStatsDataSave();
    }

}
