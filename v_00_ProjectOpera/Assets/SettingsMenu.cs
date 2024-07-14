using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    //// Flags
    //private bool valuesHaveChanged = false;
    //private bool unsavedChanges = false;

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

    //[SerializeField] private InputActionAsset controls; // Assign this in the inspector
    //private InputActionMap controls;

    // Dictionary to store settings
    private Dictionary<string, object> settingsDictionary = new();

    // Resolution options
    private Resolution[] resolutions;

    private void Start()
    {
        //controls = controls.FindActionMap("Core");
        InitializeUI();
        LoadSettings();
        ApplySettings();
        AddListenersAndDelegates();
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
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Set up other dropdowns and UI elements
        displayModeDropdown.ClearOptions();
        displayModeDropdown.AddOptions(new List<string> { "Fullscreen", "Windowed" });

        //moveUpButtonDropdown.ClearOptions();
        //moveUpButtonDropdown.AddOptions(new List<string> { "E", "SPACE" });

        //moveDownButtonDropdown.ClearOptions();
        //moveDownButtonDropdown.AddOptions(new List<string> { "Q", "LEFT SHIFT" });
    }

    private void AddListenersAndDelegates()
    {
        resolutionDropdown.onValueChanged.AddListener(OnSettingChanged);
        displayModeDropdown.onValueChanged.AddListener(OnSettingChanged);
        yFlippedToggle.onValueChanged.AddListener(OnSettingChanged);
        //moveUpButtonDropdown.onValueChanged.AddListener(OnSettingChanged);
        //moveDownButtonDropdown.onValueChanged.AddListener(OnSettingChanged);
        mouseSensitivitySlider.onValueChanged.AddListener(OnSettingChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnSettingChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSettingChanged);
    }

    private void RemoveListenersAndDelegates()
    {
        resolutionDropdown.onValueChanged.RemoveListener(OnSettingChanged);
        displayModeDropdown.onValueChanged.RemoveListener(OnSettingChanged);
        yFlippedToggle.onValueChanged.RemoveListener(OnSettingChanged);
        //moveUpButtonDropdown.onValueChanged.RemoveListener(OnSettingChanged);
        //moveDownButtonDropdown.onValueChanged.RemoveListener(OnSettingChanged);
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
        //valuesHaveChanged = true;
        //unsavedChanges = true;
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
        //settingsDictionary["MoveUpButton"] = moveUpButtonDropdown.value == 0 ? "E" : "SPACE";
        //settingsDictionary["MoveDownButton"] = moveDownButtonDropdown.value == 0 ? "Q" : "LEFT SHIFT";
        settingsDictionary["MouseSensitivity"] = mouseSensitivitySlider.value;
        settingsDictionary["MusicVolume"] = musicVolumeSlider.value;
        settingsDictionary["SFXVolume"] = sfxVolumeSlider.value;
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
        //unsavedChanges = false;
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
                    break;
                }
            }
            Debug.Log("SettingsMenu: LoadSettings: Resolution loaded!");
        }

        if (PlayerPrefs.HasKey("DisplayMode"))
        {
            displayModeDropdown.value = PlayerPrefs.GetInt("DisplayMode");
            Debug.Log("SettingsMenu: LoadSettings: DisplayMode loaded!");
        }

        if (PlayerPrefs.HasKey("YFlipped"))
        {
            yFlippedToggle.isOn = PlayerPrefs.GetInt("YFlipped") == 1;
            Debug.Log("SettingsMenu: LoadSettings: Y-flipped loaded!");
        }

        //if (PlayerPrefs.HasKey("MoveUpButton"))
        //{
        //    moveUpButtonDropdown.value = PlayerPrefs.GetString("MoveUpButton") == "E" ? 0 : 1;
        //    Debug.Log("SettingsMenu: LoadSettings: Movement Up loaded!");
        //}

        //if (PlayerPrefs.HasKey("MoveDownButton"))
        //{
        //    moveDownButtonDropdown.value = PlayerPrefs.GetString("MoveDownButton") == "Q" ? 0 : 1;
        //    Debug.Log("SettingsMenu: LoadSettings: Movement Down loaded!");
        //}

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
        FullScreenMode fullScreenMode = (FullScreenMode)settingsDictionary["DisplayMode"];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullScreenMode);

        // Apply other settings
        ApplyYFlipped((bool)settingsDictionary["YFlipped"]);
        // ApplyInputSettings((string)settingsDictionary["MoveUpButton"], (string)settingsDictionary["MoveDownButton"]);
        ApplyMouseSensitivity((float)settingsDictionary["MouseSensitivity"]);
        ApplyAudioVolumes((float)settingsDictionary["MusicVolume"], (float)settingsDictionary["SFXVolume"]);
        Debug.Log("SettingsMenu: ApplySettings: Settings applied!");
        //valuesHaveChanged = false;
        if (applyButton.activeInHierarchy == true) applyButton.SetActive(false);
    }

    // Placeholder methods for applying settings
    private void ApplyYFlipped(bool isFlipped)
    {
        // Implement y-flipped logic
    }

    //public InputActionAsset GetInputActions()
    //{
    //    return controls;
    //}

    //private void ApplyInputSettings(string moveUpButton, string moveDownButton)
    //{
    //    // Find the Move action
    //    InputAction moveAction = controls.FindAction("Movement");
    //    // Find the up and down bindings within the composite
    //    int upBindingIndex = -1;
    //    int downBindingIndex = -1;

    //    if (moveAction != null)
    //    {

    //        for (int i = 0; i < moveAction.bindings.Count; i++)
    //        {
    //            var binding = moveAction.bindings[i];
    //            if (binding.isComposite && binding.name == "3D Vector")
    //            {
    //                // Find the 'up' and 'down' part bindings
    //                for (int j = i + 1; j < moveAction.bindings.Count; j++)
    //                {
    //                    if (moveAction.bindings[j].isPartOfComposite)
    //                    {
    //                        if (moveAction.bindings[j].name == "Up")
    //                            upBindingIndex = j;
    //                        else if (moveAction.bindings[j].name == "Down")
    //                            downBindingIndex = j;

    //                        if (upBindingIndex != -1 && downBindingIndex != -1)
    //                            break;
    //                    }
    //                    else
    //                        break;
    //                }
    //                break;
    //            }
    //        }

    //        // Modify the 'up' binding
    //        if (upBindingIndex != -1)
    //        {
    //            string newUpPath = moveUpButton == "E" ? "<Keyboard>/e" : "<Keyboard>/space";
    //            moveAction.ApplyBindingOverride(upBindingIndex, newUpPath);
    //        }

    //        // Modify the 'down' binding
    //        if (downBindingIndex != -1)
    //        {
    //            string newDownPath = moveDownButton == "Q" ? "<Keyboard>/q" : "<Keyboard>/leftShift";
    //            moveAction.ApplyBindingOverride(downBindingIndex, newDownPath);
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("Movement action not found in the input action map.");
    //    }

    //    // Save the changes
    //    controls.SaveBindingOverridesAsJson();
    //    Debug.Log($"SettingsMenu: ApplyInputSettings: Input Settings applied!");

    //}

    private void ApplyMouseSensitivity(float sensitivity)
    {
        // Implement mouse sensitivity adjustment
    }

    private void ApplyAudioVolumes(float musicVolume, float sfxVolume)
    {
        // Implement audio mixer volume adjustments
    }

    // Method to get the settings dictionary (can be called by GameManager)
    public Dictionary<string, object> GetSettingsDictionary()
    {
        return settingsDictionary;
    }
}
