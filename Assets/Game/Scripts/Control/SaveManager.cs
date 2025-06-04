using System.Collections;
using UnityEngine;

public class SaveManager : MonoSingleton<SaveManager>
{
    private float lastPauseCallTime = 0f;
    public void SaveGame()
    {
        if (Time.time - lastPauseCallTime < 0.5f)
        {
            PauseManager.Instance.TogglePause();
        }
        else
        {
            var stats = GameManager.Instance.playerStats;
            var player = GameManager.Instance.playerSFM;
            stats.SetSavePoint(stats.LastSavePoint);
            player.StateMachine.ChangeState(player.saveState);
            FileSaveManager.Instance.SaveGame();
            StartCoroutine(Tooltip());
        }

        lastPauseCallTime = Time.time;
    }
    private IEnumerator Tooltip()
    {
        UIManager.Instance.ShowSaveUI();
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.HideSaveUI();
    }

}