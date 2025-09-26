using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class TypeRushManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text phraseText;
    public TMP_InputField inputField;
    public TMP_Text feedbackText;
    public TMP_Text wpmText;
    public TMP_Text accuracyText;
    public TMP_Text mistakesText;

    [Header("Phrases")]
    public List<string> phrases = new List<string>() {
        "The quick brown fox jumps over the lazy dog",
        "Unity makes game development accessible",
        "Typing games help improve hand recovery",
        "FingerFit is designed for older adults"
    };

    private string currentPhrase;
    private int currentIndex = 0;
    private int correctKeystrokes = 0;
    private int totalKeystrokes = 0;
    private int mistakeCount = 0;

    private float startTime;
    private bool timerRunning = false;

    private int lastInputLength = 0;

    void Start()
    {
        LoadNextPhrase();
        inputField.onValueChanged.AddListener(OnInputChanged);
        int fontSize = PlayerPrefs.GetInt("FontSize", 48);
        phraseText.fontSize = fontSize; // phraseText is a TMP_Text
    }

    void LoadNextPhrase()
    {
        if (currentIndex >= phrases.Count)
            currentIndex = 0; // Loop for now

        currentPhrase = phrases[currentIndex];
        phraseText.text = currentPhrase;
        inputField.text = "";
        feedbackText.text = "";
        lastInputLength = 0;
        currentIndex++;
    }
  void OnInputChanged(string userInput)
    {
        if (!timerRunning)
        {
            timerRunning = true;
            startTime = Time.time;
        }

        // Count keystrokes, but mistakes only count when typing forward
        if (userInput.Length > lastInputLength)
        {
            totalKeystrokes++;
        }

        if (userInput.Length > currentPhrase.Length)
        {
            mistakeCount++;
            feedbackText.text = "<color=red>Too many characters!</color>";
            lastInputLength = userInput.Length;
            UpdateStats();
            return;
        }

        // Build colored phrase
        int firstErrorIndex = -1;
        for (int i = 0; i < userInput.Length; i++)
        {
            if (userInput[i] != currentPhrase[i])
            {
                if (firstErrorIndex == -1)
                {
                    firstErrorIndex = i;
                }
            }
        }

        // Apply coloring
        string coloredPhrase = "";
        if (firstErrorIndex == -1)
        {
            // All correct so far message
            coloredPhrase = $"<color=green>{currentPhrase.Substring(0, userInput.Length)}</color>" +
                            currentPhrase.Substring(userInput.Length);
            feedbackText.text = "<color=green>Correct so far...</color>";
        }
        else
        {
            // Correct part up to error
            coloredPhrase = $"<color=green>{currentPhrase.Substring(0, firstErrorIndex)}</color>" +
                            $"<color=red>{currentPhrase.Substring(firstErrorIndex, userInput.Length - firstErrorIndex)}</color>" +
                            currentPhrase.Substring(userInput.Length);

            feedbackText.text = "<color=red>Incorrect!</color>";

            // Only count mistakes when typing forward
            if (userInput.Length > lastInputLength)
            {
                mistakeCount++;
            }
        }

        phraseText.text = coloredPhrase;

        if (userInput == currentPhrase)
        {
            correctKeystrokes += userInput.Length;
            inputField.text = "";
            LoadNextPhrase();
        }

        lastInputLength = userInput.Length;
        UpdateStats();
    }

    void UpdateStats()
    {
        float elapsedMinutes = (Time.time - startTime) / 60f;
        int wordsTyped = correctKeystrokes / 5; // standard formula
        int wpm = (int)(wordsTyped / elapsedMinutes);

        float accuracy = (totalKeystrokes > 0)
            ? (correctKeystrokes / (float)totalKeystrokes) * 100f
            : 0f;

        wpmText.text = $"WPM: {wpm}";
        accuracyText.text = $"Accuracy: {accuracy:F1}%";
        mistakesText.text = $"Mistakes: {mistakeCount}";
    }
}
