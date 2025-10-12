using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("UI targets to update")]
    public TMP_Text[] textTargets;      
    public Image[] backgroundImages;
    public Button[] buttonsToColor;

    [Header("Audio")]
    public AudioMixer masterMixer;
    public string mixerVolumeExposedName = "MasterVolume";

    [Header("Defaults")]
    public float defaultFontSize = 18f;
    public bool defaultHighContrast = false;
    public float defaultVolume = 0.8f;

    const string KEY_FONT = "FF_FontSize";
    const string KEY_CONTRAST = "FF_HighContrast";
    const string KEY_VOLUME = "FF_MasterVolume";

    [Header("Color Themes")]
    public string normalBgHex = "#F2F5F9";
    public string normalTextHex = "#1A1A1A";
    public string buttonHex = "#007ACC";

    public string highContrastBgHex = "#000000";
    public string highContrastTextHex = "#FFFFFF";
    public string highContrastButtonHex = "#FFD700";

    private Slider volumeSlider;
    private Slider fontSizeSlider;
    private Toggle contrastToggle;

    private bool hasInitialized = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        hasInitialized = true;
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Refresh TMP text references
        textTargets = FindObjectsOfType<TMP_Text>(true);

        // Rebind sliders/toggles each time the main menu (or a UI scene) loads
        RebindSceneControls();
    }

    // -------------------------------------------------
    // REBIND SCENE UI CONTROLS
    // -------------------------------------------------
    void RebindSceneControls()
    {
        volumeSlider = FindByTag<Slider>("SettingsVolumeSlider");
        fontSizeSlider = FindByTag<Slider>("SettingsFontSlider");
        contrastToggle = FindByTag<Toggle>("SettingsContrastToggle");

        // Clear old listeners and reattach
        if (volumeSlider)
        {
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(SetVolume);
            Debug.Log("[SettingsManager] Bound VolumeSlider");
        }

        if (fontSizeSlider)
        {
            fontSizeSlider.onValueChanged.RemoveAllListeners();
            fontSizeSlider.onValueChanged.AddListener(SetFontSize);
            Debug.Log("[SettingsManager] Bound FontSizeSlider");
        }

        if (contrastToggle)
        {
            contrastToggle.onValueChanged.RemoveAllListeners();
            contrastToggle.onValueChanged.AddListener(SetHighContrast);
            Debug.Log("[SettingsManager] Bound ContrastToggle");
        }
    }

    T FindByTag<T>(string tag) where T : Component
    {
        try
        {
            var go = GameObject.FindGameObjectWithTag(tag);
            return go ? go.GetComponent<T>() : null;
        }
        catch
        {
            Debug.LogWarning($"[SettingsManager] Tag '{tag}' not found. Please add it in Unity’s Tag Manager.");
            return null;
        }
    }

    // -------------------------------------------------
    // PUBLIC API — called by sliders, toggles, etc.
    // -------------------------------------------------
    public void SetFontSize(float size)
    {
        PlayerPrefs.SetFloat(KEY_FONT, size);
        ApplyFontSize(size);
    }

    public void SetHighContrast(bool highContrast)
    {
        PlayerPrefs.SetInt(KEY_CONTRAST, highContrast ? 1 : 0);
        ApplyTheme(highContrast);
    }

    public void SetVolume(float sliderValue)
    {
        PlayerPrefs.SetFloat(KEY_VOLUME, sliderValue);
        ApplyVolume(sliderValue);
    }

    public void ResetToDefaults()
    {
        PlayerPrefs.SetFloat(KEY_FONT, defaultFontSize);
        PlayerPrefs.SetInt(KEY_CONTRAST, defaultHighContrast ? 1 : 0);
        PlayerPrefs.SetFloat(KEY_VOLUME, defaultVolume);

        if (volumeSlider) volumeSlider.value = defaultVolume;
        if (fontSizeSlider) fontSizeSlider.value = defaultFontSize;
        if (contrastToggle) contrastToggle.isOn = defaultHighContrast;

        ApplyFontSize(defaultFontSize);
        ApplyTheme(defaultHighContrast);
        ApplyVolume(defaultVolume);
    }

    // -------------------------------------------------
    // APPLY METHODS
    // -------------------------------------------------
    void ApplyFontSize(float fontSize)
    {
        if (textTargets == null) return;

        foreach (var t in textTargets)
        {
            if (t == null || t.GetComponent<IgnoreFontScaling>() != null)
                continue;

            t.fontSize = fontSize;
        }

        Canvas.ForceUpdateCanvases();
    }

    void ApplyTheme(bool highContrast)
    {
        Color bgColor, textColor, btnColor;
        ColorUtility.TryParseHtmlString(highContrast ? highContrastBgHex : normalBgHex, out bgColor);
        ColorUtility.TryParseHtmlString(highContrast ? highContrastTextHex : normalTextHex, out textColor);
        ColorUtility.TryParseHtmlString(highContrast ? highContrastButtonHex : buttonHex, out btnColor);

        if (backgroundImages != null)
            foreach (var img in backgroundImages)
                if (img != null) img.color = bgColor;

        if (textTargets != null)
            foreach (var txt in textTargets)
                if (txt != null) txt.color = textColor;

        if (buttonsToColor != null)
            foreach (var b in buttonsToColor)
            {
                if (b == null) continue;
                var img = b.GetComponent<Image>();
                if (img != null) img.color = btnColor;
                var tm = b.GetComponentInChildren<TMP_Text>();
                if (tm != null) tm.color = textColor;
            }
    }

    void ApplyVolume(float sliderValue)
    {
        if (masterMixer != null)
        {
            float dB = (sliderValue <= 0.0001f) ? -80f : 20f * Mathf.Log10(Mathf.Clamp01(sliderValue));
            masterMixer.SetFloat(mixerVolumeExposedName, dB);
        }
        else
        {
            AudioListener.volume = Mathf.Clamp01(sliderValue);
        }
    }
}
