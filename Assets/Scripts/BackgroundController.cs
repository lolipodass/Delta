using NaughtyAttributes;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos;
    [SerializeField] private float speed;
    [Label("Camera")][SerializeField] private GameObject cam;
    void Start()
    {
        startPos = transform.position.x;
    }

    void Update()
    {
        float newPos = Mathf.Repeat(Time.time * speed, 20);
        transform.position = new Vector3(startPos + newPos, transform.position.y, transform.position.z);
    }
}
