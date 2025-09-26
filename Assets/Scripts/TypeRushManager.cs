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


    // I'm implementing a flashing red letter effect for incorrect inputs
    private bool _suppressOnChange = false;  // Prevent the OnInputChanged from immediately overwriting the "Incorrect" message
    private bool _isFlashing = false;        // Prevent overlapping flashes
    [SerializeField] private float flashDuration = 0.12f; // Flash duration in seconds

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
        currentIndex++;
    }

    void OnInputChanged(string userInput)
    {
        if (!timerRunning)
        {
            timerRunning = true;
            startTime = Time.time;
        }

        totalKeystrokes++;

        if (userInput.Length > currentPhrase.Length)
        {
            mistakeCount++;
            feedbackText.text = "<color=red>Too many characters!</color>";
            return;
        }

        for (int i = 0; i < userInput.Length; i++)
        {
            if (userInput[i] == currentPhrase[i])
                feedbackText.text = "<color=green>Correct so far...</color>";
            else
            {
                feedbackText.text = "<color=red>Incorrect!</color>";
                mistakeCount++;

                // Auto-deletes incorrect characters typed
                // The goal here is avoiding excessive mistake counts due to long wrong inputs, I found that I racked up a lot of mistakes when accidentally missing a single letter 
                // I took inspiration from the game typeracer as they incorporate a similar mechanic
                userInput = userInput.Remove(i, 1);
                inputField.text = userInput;
                inputField.caretPosition = i;


                return;
            }
        }

        if (userInput == currentPhrase)
        {
            correctKeystrokes += userInput.Length;
            inputField.text = "";
            LoadNextPhrase();
        }

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
