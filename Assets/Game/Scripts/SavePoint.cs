using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public string Name;
    public Vector3 Position;
    public GameObject Tooltip;

    public void Awake()
    {
        Name = gameObject.name;
        Position = transform.position;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.Player.GetComponent<PlayerStats>().SetLastSavePoint(this);
            if (Tooltip != null)
                Tooltip.SetActive(true);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.Player.GetComponent<PlayerStats>().SetLastSavePoint(this);
            if (Tooltip != null)
                Tooltip.SetActive(false);
        }
    }
}
