using TMPro;
using UnityEngine;

public class HealthUIManager : MonoBehaviour
{
    public TextMeshProUGUI HealthText;
    private HealthComponent _healthComponent;
    public void Awake()
    {
        // DontDestroyOnLoad(gameObject);
    }
    public void Start()
    {

        _healthComponent = GameManager.Instance.Player.GetComponent<HealthComponent>();
        _healthComponent.OnHealthChanged += UpdateHealthUI;
        UpdateHealthUI(_healthComponent.CurrentHealth);
    }
    public void UpdateHealthUI(int hp)
    {
        HealthText.text = "HP configured: " + hp;
    }
    public void OnDestroy()
    {
        if (_healthComponent != null)
        {
            _healthComponent.OnHealthChanged -= UpdateHealthUI;
        }
    }
}
