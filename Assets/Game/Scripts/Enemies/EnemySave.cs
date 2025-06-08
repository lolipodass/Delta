using UnityEngine;

public class EnemySave : SavableObject
{

    public bool IsDead { get; set; }
    public override void RestoreState(int state)
    {
        IsDead = state == 1;
    }
    public override int CaptureState()
    {
        return IsDead ? 1 : 0;
    }
    public void Save()
    {
        FileSaveManager.Instance.SaveElement(this);
    }
}
