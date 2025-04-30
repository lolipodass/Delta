using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class TimeScaleController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Input field for speed value")]
    public InputField timeScaleInput;

    [Tooltip("Button to apply the speed")]
    public Button applyButton;

    [Tooltip("Text to display the current speed (optional)")]
    public Text currentTimeScaleText;

    [Header("Settings")]
    [Tooltip("Minimum allowed TimeScale value")]
    public float minTimeScale = 0f;

    [Tooltip("Maximum allowed TimeScale value")]
    public float maxTimeScale = 5f;

    void Start()
    {

        if (timeScaleInput != null)
            timeScaleInput.text = Time.timeScale.ToString(CultureInfo.InvariantCulture);



        if (applyButton != null)
            applyButton.onClick.AddListener(ApplyTimeScale);
        else
            Debug.LogWarning("Button 'applyButton' is not assigned in TimeScaleController.");

        UpdateTimeScaleText();
    }
    public void ApplyTimeScale()
    {
        if (timeScaleInput == null)
        {
            Debug.LogError("Input field 'timeScaleInput' is not assigned in TimeScaleController.");
            return;
        }

        string inputText = timeScaleInput.text;

        // Try to convert the text from the input field to a number (float)
        // Use CultureInfo.InvariantCulture for correct parsing of numbers with a dot
        if (float.TryParse(inputText, NumberStyles.Any, CultureInfo.InvariantCulture, out float newTimeScale))
        {
            newTimeScale = Mathf.Clamp(newTimeScale, minTimeScale, maxTimeScale);
            Time.timeScale = newTimeScale;
            timeScaleInput.text = newTimeScale.ToString(CultureInfo.InvariantCulture);

            UpdateTimeScaleText();

            Debug.Log($"Game speed changed to: {Time.timeScale}");
        }
        else
        {
            Debug.LogWarning($"Failed to convert '{inputText}' to a number. Please enter a correct speed value.");
            timeScaleInput.text = Time.timeScale.ToString(CultureInfo.InvariantCulture);
        }
    }
    void UpdateTimeScaleText()
    {
        if (currentTimeScaleText != null)
        {
            // Use CultureInfo.InvariantCulture for correct display of the dot
            currentTimeScaleText.text = $"{Time.timeScale.ToString("F2", CultureInfo.InvariantCulture)}";
        }
    }

    // Important! Reset Time.timeScale when exiting the scene or stopping the game,
    // so as not to affect other scenes or the editor.
    void OnDestroy()
    {
        // Reset Time.timeScale to the default value (1.0) when the object is destroyed
        // This is important so that changes are not saved between game runs in the editor
        // or when switching to another scene, if this controller is not persistent.
        Time.timeScale = 1.0f;
    }
}