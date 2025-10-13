using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    public static Settings Instance;

    [Header("Audio")]
    public AudioMixer masterMixer;
    [Range(0f, 1f)] public float masterVolume = 1f;

    [Header("UI Settings")]
    public float fontSize = 28f;
    public bool highContrast = false;

    [Header("Colors")]
    public Color normalBgColor = Color.white;
    public Color normalTextColor = Color.black;
    public Color contrastBgColor = Color.black;
    public Color contrastTextColor = Color.yellow;

    [Header("UI References (in Settings Scene only)")]
    public Slider volumeSlider;
    public Slider fontSizeSlider;
    public Toggle contrastToggle;

    private bool hasUserChangedSettings = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        UpdateUIControls(); // set sliders/toggles to reflect current session state
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateUIControls(); // update controls if we return to settings scene

        // Only apply settings if user has changed something this session
        if (hasUserChangedSettings)
            ApplyAllSettings();
    }

    // -------------------------
    // APPLY SETTINGS
    // -------------------------
    public void ApplyAllSettings()
    {
        ApplyVolume(masterVolume);

        if (hasUserChangedSettings) // only apply font & contrast if user changed
        {
            ApplyFontSize(fontSize);
            ApplyContrast(highContrast);
        }
    }

    public void ApplyVolume(float value)
    {
        masterVolume = value;
        if (masterMixer != null)
        {
            float db = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
            masterMixer.SetFloat("MasterVolume", db);
        }
        else
        {
            AudioListener.volume = value;
        }
    }

    public void ApplyFontSize(float size)
    {
        fontSize = size;
        hasUserChangedSettings = true;

        foreach (TMP_Text txt in FindObjectsOfType<TMP_Text>(true))
        {
            if (txt.GetComponent<IgnoreFontScaling>() == null)
                txt.fontSize = size;
        }
    }

    public void ApplyContrast(bool state)
    {
        highContrast = state;
        hasUserChangedSettings = true;

        Color bg = highContrast ? contrastBgColor : normalBgColor;
        Color text = highContrast ? contrastTextColor : normalTextColor;

        foreach (TMP_Text txt in FindObjectsOfType<TMP_Text>(true))
            txt.color = text;

        foreach (Image img in FindObjectsOfType<Image>(true))
            img.color = bg;
    }

    // -------------------------
    // UI HOOKS
    // -------------------------
    public void OnVolumeChanged(float value)
    {
        hasUserChangedSettings = true;
        ApplyVolume(value);
    }

    public void OnFontSizeChanged(float value)
    {
        hasUserChangedSettings = true;
        ApplyFontSize(value);
    }

    public void OnContrastToggled(bool value)
    {
        hasUserChangedSettings = true;
        ApplyContrast(value);
    }

    void UpdateUIControls()
    {
        if (volumeSlider) volumeSlider.value = masterVolume;
        if (fontSizeSlider) fontSizeSlider.value = fontSize;
        if (contrastToggle) contrastToggle.isOn = highContrast;
    }

    public void BackToPreviousScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
