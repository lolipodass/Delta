using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsManager : MonoSingleton<SettingsManager>
{
    public AudioMixer MainMixer;
    public AudioMixer SFXMixer;
    public AudioMixer MusicMixer;

    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;
    public Slider VolumeSlider;
    private Resolution[] resolutions;
    public TMPro.TMP_Dropdown resolutionDropdown;
    public TMPro.TMP_Dropdown qualityDropdown;
    public Resolution CurrentResolution;
    protected override void Awake()
    {
        base.Awake();
        resolutions = Screen.resolutions;
        CurrentResolution = resolutions[0];


        int currentResolutionIndex = 0;
        resolutionDropdown.ClearOptions();
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionDropdown.options.Add(new(resolutions[i].width.ToString() + " x " + resolutions[i].height.ToString()));
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                CurrentResolution = resolutions[i];
                currentResolutionIndex = i;
            }
        }
        Debug.Log($"Current resolution: {CurrentResolution.width} x {CurrentResolution.height}");
        Debug.Log($"Current resolution index: {currentResolutionIndex}");

        // SFXVolumeSlider.value = SFXMixer.GetFloat("SFXVolume");
        // VolumeSlider.value = MainMixer.GetFloat("Volume");

        resolutionDropdown.value = currentResolutionIndex;
        qualityDropdown.value = QualitySettings.GetQualityLevel();

        GameManager.Instance.playerInput.actions.FindAction("Pause").performed += PauseCallback;
    }

    protected void OnDestroy()
    {
        if (GameManager.Instance.playerInput != null)
        {
            GameManager.Instance.playerInput.actions.FindAction("Pause").performed -= PauseCallback;
        }
    }
    public void PauseCallback(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneLoader.UnloadSettings();
        }
    }

    public void ChangeResolution(Resolution resolution)
    {
        Screen.SetResolution(resolution.width, resolution.height, false);
    }
    public void ChangeVolume(float volume)
    {
        MainMixer.SetFloat("Volume", volume);
    }
    public void ChangeMusicVolume(float volume)
    {
        MusicMixer.SetFloat("MusicVolume", volume);
    }
    public void ChangeSFXVolume(float volume)
    {
        SFXMixer.SetFloat("SFXVolume", volume);
    }
    public void SetFullScreen(bool fullScreen)
    {
        Screen.fullScreen = fullScreen;
    }

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, false);
    }
    public void SetQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
    }
    public void SetResolution(int id)
    {
        CurrentResolution = resolutions[id];
        ChangeResolution(CurrentResolution);
    }
}
