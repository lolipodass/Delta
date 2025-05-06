using System.Collections.Generic;
using UnityEngine;

public class GameDataSave
{

    public class PlayerStatsDataSave
    {
        public List<UpgradeModifier> activePlayerModifiers;
        public int HP;
    }

    public PlayerStatsDataSave playerStatsDataSave;

    public GameDataSave()
    {
        playerStatsDataSave = new PlayerStatsDataSave();
    }

}
