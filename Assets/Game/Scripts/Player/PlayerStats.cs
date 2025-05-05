using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerStatsManager StatsManager;
    public PlayerBaseInfo PlayerConfig;
    public void Awake()
    {
        StatsManager = new PlayerStatsManager(PlayerConfig);
    }
    //!load to file and from file
}
