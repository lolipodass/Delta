using Cysharp.Threading.Tasks;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // await UniTask.Delay(System.TimeSpan.FromSeconds(2f));
            Destroy(gameObject);
            GameManager.Instance.Finish();
        }
    }
}
