using System.Collections;
using UnityEngine;

public class DeathManager : MonoSingleton<DeathManager>
{
    protected override void Awake()
    {
        base.Awake();

    }
    public void HandleDeath()
    {
        StartCoroutine(ShowUI());
    }

    private IEnumerator ShowUI()
    {
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.ShowDeathUI();
        yield return new WaitForSeconds(1f);
        GameManager.Instance.MovePlayerToSavePoint();
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.HideDeathUI();
        GameManager.Instance.playerSFM.Restart();
    }
}
