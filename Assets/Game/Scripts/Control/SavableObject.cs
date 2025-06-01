using System;
using UnityEngine;

public class SavableObject : MonoBehaviour
{
    [SerializeField] private Ulid id;

    public Ulid Id { get => id; set => id = value; }

    [ContextMenu("Generate New ID")]
    private void GenerateId()
    {
        id = Ulid.NewUlid();
        Debug.Log($"Generated new ID for {gameObject.name}: {id}");
    }

    protected virtual void Awake()
    {
        if (id.CompareTo(Ulid.Empty) == 0)
        {
            GenerateId();
        }

        FileSaveManager.Instance.LoadElement(this);
    }
    public virtual int CaptureState()
    {
        return 0;
    }

    public virtual void RestoreState(int state)
    {
    }

}