using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SequenceSparkManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text phraseText;
    public TMP_InputField inputField;
    public TMP_Text feedbackText;
    public TMP_Text scoreText;
    public TMP_Text currentStreakText;
    public TMP_Text longestStreakText;
    public TMP_Text mistakesText;
    public GameObject gameOverPanel;

    [Header("Game Rules")]
    public int mistakeLimit = 3;
    public int maxRounds = 0;              // 0 = endless
    public bool reflashOnFailure = true;
    public bool showInputBullets = true;

    [Header("Sequence Settings")]
    public string allowedCharacters = "asdfjkl;ghqwertyuiopzxcvbnm";
    public bool forceLowercase = true;

    [Header("Flash Timing")]
    public float flashDuration = 0.45f;
    public float interLetterDelay = 0.2f;
    public float roundDelay = 0.8f;

    [Header("Scoring")]
    public int pointsPerRound = 10;
    public int pointsPerChar = 1;

    private readonly List<char> sequence = new List<char>();
    private int roundNumber = 0;

    private int totalScore = 0;
    private int currentStreak = 0;
    private int longestStreak = 0;
    private int mistakeCount = 0;

    private bool gameOver = false;
    private bool showingSequence = false;

    private string sanitizedInput = "";
    private int lastInputLength = 0;

    private void Start()
    {
        inputField.onValueChanged.AddListener(OnInputChanged);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        int fontSize = PlayerPrefs.GetInt("FontSize", 48);
        if (phraseText) phraseText.fontSize = fontSize;

        BeginFirstRun();
    }

    private void BeginFirstRun()
    {
        // reset all runtime values
        sequence.Clear();
        roundNumber = 0;
        totalScore = 0;
        currentStreak = 0;
        longestStreak = 0;
        mistakeCount = 0;
        lastInputLength = 0;

        gameOver = false;
        showingSequence = false;

        // reset UI
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (phraseText) phraseText.text = "";
        if (feedbackText) feedbackText.text = "";
        if (inputField)
        {
            inputField.text = "";
            inputField.interactable = false;
        }

        UpdateStatsUI();
        StartCoroutine(BeginNextRound());
    }

    private IEnumerator BeginNextRound()
    {
        if (gameOver) yield break;

        phraseText.text = "";

        roundNumber++;
        AppendRandomLetter();

        inputField.text = string.Empty;
        phraseText.text = "";
        lastInputLength = 0;

        if (feedbackText) feedbackText.text = $"<color=#87CEFA>Round {roundNumber}</color>";
        yield return new WaitForSeconds(roundDelay);

        // Flash sequence
        yield return StartCoroutine(FlashSequence());

        // Ready for input
        showingSequence = false;

        
        

        inputField.interactable = true;
        inputField.ActivateInputField();

        // Initialize Prompt
        phraseText.text = "Type the sequence" + "\n\n" + BuildBulletPlaceholders("", sequence.Count);
    }

    private void AppendRandomLetter()
    {
        if (string.IsNullOrEmpty(allowedCharacters))
            allowedCharacters = "abcdefghijklmnopqrstuvwxyz";

        int idx = Random.Range(0, allowedCharacters.Length);
        char c = allowedCharacters[idx];
        sequence.Add(forceLowercase ? char.ToLowerInvariant(c) : c);
    }

    private IEnumerator FlashSequence()
    {
        showingSequence = true;
        inputField.interactable = false;

        if (phraseText) phraseText.text = "";

        yield return new WaitForSeconds(0.15f);

        for (int i = 0; i < sequence.Count; i++)
        {
            char c = sequence[i];
            string shown = forceLowercase ? c.ToString().ToLowerInvariant() : c.ToString();

            // Flash a single letter
            if (phraseText) phraseText.text = $"<size=140%><b>{shown}</b></size>";
            yield return new WaitForSeconds(flashDuration);

            // Gap between letters
            if (phraseText) phraseText.text = "";
            yield return new WaitForSeconds(interLetterDelay);
        }
    }

    private void OnInputChanged(string rawInput)
    {
        if (inputField.interactable == false) return;

        if (gameOver || showingSequence)
        {
            phraseText.text = "";
            return;
        }

        sanitizedInput = forceLowercase ? rawInput.ToLowerInvariant() : rawInput;

        if (showInputBullets && phraseText)
        {
            phraseText.text = "Type the sequence" + "\n\n" + BuildBulletPlaceholders(sanitizedInput, sequence.Count);
        }

        if (sanitizedInput.Length > sequence.Count)
        {
            HandleRoundFail("<color=red>Too many characters!</color>");
            return;
        }

        for (int i = 0; i < sanitizedInput.Length; i++)
        {
            if (sanitizedInput[i] != sequence[i])
            {
                HandleRoundFail("<color=red>Incorrect!</color>");
                return;
            }
        }

        // completed full input correctly
        if (sanitizedInput.Length == sequence.Count)
        {
            HandleRoundSuccess();
        }

        lastInputLength = sanitizedInput.Length;
    }

    private string BuildBulletPlaceholders(string typedInput, int totalLength)
    {
        if (totalLength > 100) totalLength = 100;
        var sb = new StringBuilder(totalLength * 2);

        for (int i = 0; i < totalLength; i++)
        {
            if (i > 0) sb.Append(' ');

            if (i < typedInput.Length)
            {
                sb.Append(typedInput[i]);
            }
            else
            {
                sb.Append("<alpha=#60>•");
            }
        }

        return sb.ToString();
    }


    private void HandleRoundSuccess()
    {
        inputField.interactable = false;
        int points = pointsPerRound + (pointsPerChar * sequence.Count);
        totalScore += points;

        currentStreak++;
        if (currentStreak > longestStreak) longestStreak = currentStreak;

        UpdateStatsUI();

        // Win condition if capped rounds
        if (maxRounds > 0 && roundNumber >= maxRounds)
        {
            if (feedbackText) feedbackText.text = "<color=#00FF7F>Sequence Spark Complete!</color>";
            EndGame();
            return;
        }

        if (feedbackText) feedbackText.text = "<color=#00FF7F>Nice! +" + points + " pts</color>";

        // progress to next round
        StartCoroutine(BeginNextRound());
    }

    private void HandleRoundFail(string reason)
    {
        mistakeCount++;
        currentStreak = 0;
        UpdateStatsUI();

        if (feedbackText) feedbackText.text = reason;

        if (mistakeCount >= mistakeLimit)
        {
            if (feedbackText) feedbackText.text = "<color=red>Game Over! Too many mistakes.</color>";
            EndGame();
            return;
        }

        inputField.text = "";
        lastInputLength = 0;

        if (reflashOnFailure)
        {
            StartCoroutine(ReflashSameSequence());
        }
        else
        {
            // If not reflashing, prompt to try again
            if (phraseText) phraseText.text = "<alpha=#60>Try again…</alpha>";
            inputField.ActivateInputField();
        }
    }

    private IEnumerator ReflashSameSequence()
    {
        inputField.interactable = false;
        yield return new WaitForSeconds(0.6f);
        if (feedbackText) feedbackText.text = "<color=#FFD700>Watch closely…</color>";
        yield return StartCoroutine(FlashSequence());
        showingSequence = false;
        if (phraseText) phraseText.text = "<alpha=#60>Type the sequence…</alpha>";
        inputField.interactable = true;
        inputField.ActivateInputField();
    }

    private void UpdateStatsUI()
    {
        if (scoreText) scoreText.text = $"Score: {totalScore}";
        if (currentStreakText) currentStreakText.text = $"Streak: {currentStreak}";
        if (longestStreakText) longestStreakText.text = $"Best: {longestStreak}";
        if (mistakesText) mistakesText.text = $"Mistakes: {mistakeCount}/{mistakeLimit}";
    }

    private void EndGame()
    {
        gameOver = true;
        showingSequence = false;
        inputField.interactable = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }
    public void Restart()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        BeginFirstRun();
    }
}
