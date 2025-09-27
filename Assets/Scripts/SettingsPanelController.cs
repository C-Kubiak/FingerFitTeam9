using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SettingsPanelController : MonoBehaviour
{
    [Header("Panel")]
    public GameObject settingsPanel;         // root GameObject of panel
    public CanvasGroup panelCanvasGroup;     // CanvasGroup on panel (for fade)
    public float animDuration = 0.18f;

    [Header("Background block")]
    public CanvasGroup menuCanvasGroup;      // main menu's CanvasGroup to disable when panel open

    void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }
    public void TogglePanel()
    {
        if (settingsPanel == null) return;

        //Toggle panel active state
        if (!settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(true);
            StartCoroutine(FadeIn());
        }

        //Toggle panel inactive state
        else if (settingsPanel.activeSelf)
        {
            StartCoroutine(FadeOut());
        }


    }

    public void ClosePanel()
    {
        if (settingsPanel == null) return;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = true;
        if (menuCanvasGroup != null) menuCanvasGroup.interactable = false;

        float t = 0f;
        while (t < animDuration)
        {
            t += Time.unscaledDeltaTime;
            panelCanvasGroup.alpha = Mathf.Clamp01(t / animDuration);
            yield return null;
        }

        panelCanvasGroup.alpha = 1f;
        panelCanvasGroup.interactable = true;
        // optionally set default selected UI element:
        EventSystem.current.SetSelectedGameObject(null);
    }

    IEnumerator FadeOut()
    {
        panelCanvasGroup.interactable = false;
        if (menuCanvasGroup != null) menuCanvasGroup.interactable = true;

        float t = animDuration;
        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime;
            panelCanvasGroup.alpha = Mathf.Clamp01(t / animDuration);
            yield return null;
        }

        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.blocksRaycasts = false;
        settingsPanel.SetActive(false);
    }
}
