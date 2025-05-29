using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class GameDataSave
{
    [MemoryPackable]
    [System.Serializable]
    public partial class PlayerStatsDataSave
    {
        public string[] items;
        public int HP;
        public SavePointInfo savePoint;
        public PlayerStatsDataSave()
        {
            items = new string[0];
            HP = 0;
            savePoint = new SavePointInfo();
        }
        [MemoryPackable]

        [System.Serializable]
        public partial class SavePointInfo
        {
            public string Name;
            public Vector3 Position;
            public SavePointInfo()
            {
                Name = "";
                Position = Vector3.zero;
            }
            [MemoryPackConstructor]
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
