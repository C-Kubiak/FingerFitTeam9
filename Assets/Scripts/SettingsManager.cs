using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("UI targets to update")]
    public TMP_Text[] textTargets;      // drag TMP_Text objects here in Inspector
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

    public string normalBgHex = "#F2F5F9";
    public string normalTextHex = "#1A1A1A";
    public string buttonHex = "#007ACC";

    public string highContrastBgHex = "#000000";
    public string highContrastTextHex = "#FFFFFF";
    public string highContrastButtonHex = "#FFD700";


    [Header("UI Object Settings (for reset values)")]
    public GameObject volumeSliderGO;
    public GameObject fontSizeSliderGO;
    public GameObject contrastToggleGO;

    private Slider volumeSlider;
    private Slider fontSizeSlider;
    private Toggle contrastToggle;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // survive scene loads
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // Cache the Slider components from the GameObjects
        volumeSlider = volumeSliderGO.GetComponent<Slider>();
        fontSizeSlider = fontSizeSliderGO.GetComponent<Slider>();
        contrastToggle = contrastToggleGO.GetComponent<Toggle>();


        LoadAndApplyAll();
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // find all TMP_Text in this scene
        var foundTexts = FindObjectsOfType<TMP_Text>(true);
        textTargets = foundTexts;

        // reapply settings to them
        LoadAndApplyAll();
    }

    #region Public API for UI bindings
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

        // Refresh UI Sliders
        volumeSlider.value = SettingsManager.Instance.defaultVolume;
        fontSizeSlider.value = SettingsManager.Instance.defaultFontSize;
        contrastToggle.isOn = SettingsManager.Instance.defaultHighContrast;


        LoadAndApplyAll();
    }
    #endregion

    void LoadAndApplyAll()
    {
        float font = PlayerPrefs.HasKey(KEY_FONT) ? PlayerPrefs.GetFloat(KEY_FONT) : defaultFontSize;
        bool hc = PlayerPrefs.HasKey(KEY_CONTRAST) ? (PlayerPrefs.GetInt(KEY_CONTRAST) == 1) : defaultHighContrast;
        float vol = PlayerPrefs.HasKey(KEY_VOLUME) ? PlayerPrefs.GetFloat(KEY_VOLUME) : defaultVolume;

        ApplyFontSize(font);
        ApplyTheme(hc);
        ApplyVolume(vol);
    }

    void ApplyFontSize(float fontSize)
    {
        if (textTargets == null) return;
        foreach (var t in textTargets)
        {
            if (t == null) continue;

            // NEW: Skip texts with IgnoreFontScaling
            if (t.GetComponent<IgnoreFontScaling>() != null)
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
            foreach (var img in backgroundImages) if (img != null) img.color = bgColor;

        if (textTargets != null)
            foreach (var txt in textTargets) if (txt != null) txt.color = textColor;

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

    // Public getters
    public float CurrentFontSize => PlayerPrefs.GetFloat(KEY_FONT, defaultFontSize);
    public bool IsHighContrast => PlayerPrefs.GetInt(KEY_CONTRAST, defaultHighContrast ? 1 : 0) == 1;
}
