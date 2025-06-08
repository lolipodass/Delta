using System;
using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

[System.Serializable]
[MemoryPackable]
public partial class GameDataSave
{
    public PlayerStatsDataSave player;
    public Dictionary<Ulid, int> saveAbles;

    public GameDataSave()
    {
        player = new PlayerStatsDataSave();
        saveAbles = new Dictionary<Ulid, int>();
    }

    [MemoryPackable]
    [System.Serializable]
    public partial class PlayerStatsDataSave
    {
        public string[] items;
        public int HP;
        public int score;
        public int deaths;
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

            public Vector3Save Position;
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


}

public struct Vector3Save
{
    public float x;
    public float y;
    public float z;

    public Vector3Save(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
    public static implicit operator Vector3Save(Vector3 v)
    {
        return new Vector3Save(v);
    }
    public static implicit operator Vector3(Vector3Save v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

}