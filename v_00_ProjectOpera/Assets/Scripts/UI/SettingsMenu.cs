using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // UI References
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown displayModeDropdown;
    [SerializeField] private Toggle yFlippedToggle;
    [SerializeField] private TMP_Dropdown moveUpButtonDropdown;
    [SerializeField] private TMP_Dropdown moveDownButtonDropdown;
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private GameObject applyButton;
    [SerializeField] private GameObject saveButton;

    // Dictionary to store settings
    public Dictionary<string, object> settingsDictionary = new();

    // Resolution options
    private Resolution[] resolutions;

    private void Start()
    {
        AddListenersAndDelegates();
        InitializeUI();
        if (GameManager.gameJustStarted)
        {
            LoadSettings();
            //ApplySettings();
            GameManager.gameJustStarted = true;
        }
    }

    private void InitializeUI()
    {
        // Populate resolution dropdown
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            if (options.Contains(option) == false) options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Set up display mode dropdown
        displayModeDropdown.ClearOptions();
        displayModeDropdown.AddOptions(new List<string> { "Fullscreen", "Windowed" });

        // Determine the current screen mode and set the dropdown value
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow || Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            Debug.Log("DisplayMode was Fullscreen!");
            displayModeDropdown.value = 0; // Fullscreen
        }
        else if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            Debug.Log("DisplayMode was Windowed!");
            displayModeDropdown.value = 1; // Windowed
        }

        // Refresh the shown value to update the dropdown display
        displayModeDropdown.RefreshShownValue();


    }

    private void AddListenersAndDelegates()
    {
        resolutionDropdown.onValueChanged.AddListener(OnSettingChanged);
        displayModeDropdown.onValueChanged.AddListener(OnSettingChanged);
        yFlippedToggle.onValueChanged.AddListener(OnSettingChanged);
        mouseSensitivitySlider.onValueChanged.AddListener(OnSettingChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnSettingChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSettingChanged);
    }

    private void RemoveListenersAndDelegates()
    {
        resolutionDropdown.onValueChanged.RemoveListener(OnSettingChanged);
        displayModeDropdown.onValueChanged.RemoveListener(OnSettingChanged);
        yFlippedToggle.onValueChanged.RemoveListener(OnSettingChanged);
        mouseSensitivitySlider.onValueChanged.RemoveListener(OnSettingChanged);
        musicVolumeSlider.onValueChanged.RemoveListener(OnSettingChanged);
        sfxVolumeSlider.onValueChanged.RemoveListener(OnSettingChanged);
    }

    private void OnSettingChanged(float value)
    {
        OnSettingChanged();
    }

    private void OnSettingChanged(bool value)
    {
        OnSettingChanged();
    }

    private void OnSettingChanged(int value)
    {
        OnSettingChanged();
    }

    private void OnSettingChanged()
    {
        UpdateSettingsDictionary();
        if (applyButton.activeInHierarchy == false) applyButton.SetActive(true);
        if (saveButton.activeInHierarchy == false) saveButton.SetActive(true);
    }

    private void OnDisable()
    {
        RemoveListenersAndDelegates();
    }

    private void OnDestroy()
    {
        RemoveListenersAndDelegates();
    }

    private void UpdateSettingsDictionary()
    {
        settingsDictionary["Resolution"] = resolutions[resolutionDropdown.value];
        settingsDictionary["DisplayMode"] = displayModeDropdown.value == 0 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        settingsDictionary["YFlipped"] = yFlippedToggle.isOn;
        settingsDictionary["MouseSensitivity"] = mouseSensitivitySlider.value;
        settingsDictionary["MusicVolume"] = musicVolumeSlider.value;
        settingsDictionary["SFXVolume"] = sfxVolumeSlider.value;

        GameManager.Instance.UpdateSettingsDictionary(settingsDictionary);
    }

    public void SaveSettings()
    {
        foreach (var setting in settingsDictionary)
        {
            if (setting.Value is not Resolution res)
            {
                if (setting.Value is FullScreenMode)
                {
                    PlayerPrefs.SetInt(setting.Key, (int)setting.Value);
                }
                else if (setting.Value is bool boolean)
                {
                    PlayerPrefs.SetInt(setting.Key, boolean ? 1 : 0);
                }
                else if (setting.Value is string @string)
                {
                    PlayerPrefs.SetString(setting.Key, @string);
                }
                else if (setting.Value is float single)
                {
                    PlayerPrefs.SetFloat(setting.Key, single);
                }
            }
            else
            {
                PlayerPrefs.SetInt(setting.Key + "Width", res.width);
                PlayerPrefs.SetInt(setting.Key + "Height", res.height);
            }
        }

        PlayerPrefs.Save();
        Debug.Log("SettingsMenu: SaveSettings: PlayerPrefs saved!");
        if (saveButton.activeInHierarchy == true) saveButton.SetActive(false);
    }

    private void LoadSettings()
    {
        Debug.Log("SettingsMenu: LoadSettings: Trying to load PlayerPrefs.");
        // Load and apply saved settings
        if (PlayerPrefs.HasKey("ResolutionWidth") && PlayerPrefs.HasKey("ResolutionHeight"))
        {
            int width = PlayerPrefs.GetInt("ResolutionWidth");
            int height = PlayerPrefs.GetInt("ResolutionHeight");
            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].width == width && resolutions[i].height == height)
                {
                    resolutionDropdown.value = i;
                    resolutionDropdown.RefreshShownValue();
                    break;
                }
            }
            Debug.Log("SettingsMenu: LoadSettings: Resolution loaded!");
        }

        if (PlayerPrefs.HasKey("DisplayMode"))
        {
            displayModeDropdown.value = PlayerPrefs.GetInt("DisplayMode");
            displayModeDropdown.RefreshShownValue();
            Debug.Log("SettingsMenu: LoadSettings: DisplayMode loaded!");
        }

        if (PlayerPrefs.HasKey("YFlipped"))
        {
            yFlippedToggle.isOn = PlayerPrefs.GetInt("YFlipped") == 1;
            Debug.Log("SettingsMenu: LoadSettings: Y-flipped loaded!");
        }

        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
            Debug.Log("SettingsMenu: LoadSettings: Mouse Sensitivity loaded!");
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            Debug.Log("SettingsMenu: LoadSettings: Music Volume loaded!");
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            Debug.Log("SettingsMenu: LoadSettings: SFX Volume loaded!");
        }

        UpdateSettingsDictionary();
        Debug.Log("SettingsMenu: LoadSettings: PlayerPrefs loaded!");
    }

    public void ApplySettings()
    {
        // Apply resolution and display mode
        Resolution selectedResolution = (Resolution)settingsDictionary["Resolution"];

        int displayModeIndex = (int)settingsDictionary["DisplayMode"];
        FullScreenMode fullScreenMode = (FullScreenMode)displayModeIndex;
        Debug.Log($"SettingsMenu: ApplySettings: Fullwscreen mode is {fullScreenMode}!");

        Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullScreenMode);

        Debug.Log("SettingsMenu: ApplySettings: Settings applied!");
        if (applyButton.activeInHierarchy == true) applyButton.SetActive(false);
    }
}
