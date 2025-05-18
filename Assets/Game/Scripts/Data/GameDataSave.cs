using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDataSave
{

    [System.Serializable]
    public class PlayerStatsDataSave
    {
        public PlayerStatsDataSave()
        {
            items = new string[0];
            HP = 0;
            savePoint = new SavePointInfo();
        }
        public string[] items;
        public int HP;
        public SavePointInfo savePoint;
        [System.Serializable]
        public class SavePointInfo
        {
            public string Name;
            public Vector3 Position;
            public SavePointInfo()
            {
                Name = "";
                Position = Vector3.zero;
            }

            public SavePointInfo(string name, Vector3 position)
            {
                Name = name;
                Position = position;
            }
            public SavePointInfo(SavePoint savePoint)
            {
                Name = savePoint.Name;
                Position = savePoint.Position;
            }
        }
    }

    public PlayerStatsDataSave player;

    public GameDataSave()
    {
        player = new PlayerStatsDataSave();

    }

}
