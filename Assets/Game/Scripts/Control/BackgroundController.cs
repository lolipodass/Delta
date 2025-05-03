using NaughtyAttributes;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos;
    private float SpriteLength;
    [Range(0, 1f)][SerializeField] private float ParallaxEffect;
    [Label("Camera")][SerializeField] private GameObject cam;
    void Start()
    {
        startPos = transform.position.x;
        SpriteLength = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        float cameraPosX = cam.transform.position.x;
        float newPos = cameraPosX * (1 - ParallaxEffect);
        float SpritePos = cameraPosX * ParallaxEffect;

        if (SpritePos > startPos + SpriteLength)
        {
            startPos += SpriteLength;
        }
        else if (SpritePos < startPos - SpriteLength)
        {
            startPos -= SpriteLength;
        }

        transform.position = new(startPos + newPos, cam.transform.position.y);
    }
}
