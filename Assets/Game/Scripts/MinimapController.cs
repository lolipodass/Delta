using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoSingleton<MinimapController>
{

    public Image minimap;
    public RectTransform indicator;
    public Vector3 centerOffset = Vector2.zero;
    public float minimapScale = 1.4f;

    protected override void Awake()
    {
        base.Awake();
        centerOffset = indicator.anchoredPosition;
        Debug.Log("centerOffset: " + centerOffset);
        indicator.transform.localPosition = new Vector3(0, 0, 0);
        UIManager.Instance.OnShowPauseMenu += UpdateIndicator;
    }

    public void UpdateIndicator()
    {

        var playerPosition = GameManager.Instance.playerSFM.transform.position;
        var indicatorPosition = (playerPosition + centerOffset) * minimapScale;
        indicatorPosition.z = 0;
        indicator.anchoredPosition = indicatorPosition;
        Debug.Log("indicator position: " + indicator.transform.localPosition);
    }
}
