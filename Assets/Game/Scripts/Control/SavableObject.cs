using System;
using UnityEngine;

public class SavableObject : MonoBehaviour
{

    [SerializeField, Ulid] private string _ulidString;

    public Ulid Id
    {
        get
        {
            if (string.IsNullOrEmpty(_ulidString) || !Ulid.TryParse(_ulidString, out Ulid parsedUlid))
            {
                return Ulid.Empty;
            }
            return parsedUlid;
        }
        set
        {
            _ulidString = value.ToString();
        }

    }


    protected virtual void Awake()
    {

        if (Id.CompareTo(Ulid.Empty) == 0)
        {
            Debug.LogError($"SavableObject {gameObject.name} has empty ID!", this);
            gameObject.SetActive(false);
            return;
            //     GenerateId();
        }

        if (!FileSaveManager.Instance.LoadElement(this))
        {
            FileSaveManager.Instance.OnGameLoaded += OnGameLoaded;
        }
    }
    private void OnGameLoaded()
    {
        FileSaveManager.Instance.LoadElement(this);
        FileSaveManager.Instance.OnGameLoaded -= OnGameLoaded;
    }
    public virtual int CaptureState()
    {
        return 0;
    }

    public virtual void RestoreState(int state)
    {
    }

}