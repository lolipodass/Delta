using UnityEngine;

[CreateAssetMenu(fileName = "NewEffect", menuName = "Game Effects/Effect Data")]
public class EffectData : ScriptableObject
{
    public string effectName;

    [Header("Particle Effect")]
    public GameObject particlePrefab;
    public bool useParticlePooling = true;

    [Header("Sound Effect")]
    public AudioClip soundClip;
    [Range(0f, 1f)] public float volume = 1.0f;

    [Header("Camera Shake")]
    public bool shakeCamera = false;
    public float shakeDuration = 0.3f;
    public float shakeIntensity = 0.5f;

}