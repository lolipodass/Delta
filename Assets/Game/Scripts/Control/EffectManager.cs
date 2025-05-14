
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class EffectManager : PersistSingleton<EffectManager>
{
    [SerializeField] private List<EffectData> availableEffects;


    public void PlayEffect(EffectData effectData)
    {
        Debug.Log("PlayEffect");
        if (effectData == null) return;
        if (effectData.soundClip != null)
        {
            AudioSource.PlayClipAtPoint(effectData.soundClip, transform.position);
        }
        if (effectData.particlePrefab != null)
        {
            Instantiate(effectData.particlePrefab, transform.position, Quaternion.identity);
        }
        if (effectData.shakeCamera)
        {
            Tween.ShakeCamera(GameManager.Instance.Camera, effectData.shakeIntensity, effectData.shakeDuration);
        }
    }

}