using UnityEngine;
using TMPro;
using UnityEngine.UI;
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
    [SerializeField, TextArea(2, 5)]
    private List<string> phrases = new List<string>() {
        "The quick brown fox jumps over the lazy dog",
        "Typing games help improve hand recovery",
        "Unity makes game development accessible",
        "FingerFit is designed for older adults",
        "Practice makes perfect when typing every day",
        "Small consistent steps lead to big results",
        "Healthy hands mean more freedom in daily life",
        "Good posture helps with comfortable typing",
        "Never give up on learning new skills",
        "Patience and practice improve coordination",
        "Typing trains both the brain and the hands",
        "Every letter typed builds muscle memory",
        "Speed comes naturally after accuracy",
        "Focus on progress, not perfection",
        "Typing is a useful everyday skill",
        "Stay calm and keep practicing typing",
        "The journey is just as important as the goal",
        "Typing exercises help strengthen fingers",
        "Balance speed with accuracy for success",
        "Typing with rhythm can improve flow",
        // ... add up to 100+ phrases ...
        "Always look ahead to the next challenge",
        "Practice typing with joy and patience",
        "Train your hands like an athlete trains muscles",
        "Typing improves both memory and focus",
        "Recovery takes time but effort pays off",
        "Each keystroke brings you closer to mastery",
        "Stay positive and consistent with training",
        "Typing is a gateway to digital communication",
        "Fast fingers make computer use easier",
        "Strength grows when effort is repeated",
        "Every challenge is a chance to grow",
        "Practice typing in short focused sessions",
        "Typing strengthens hand-eye coordination",
        "Repetition builds familiarity and skill",
        "Accuracy first, speed second, mastery third",
        "Typing helps sharpen mental agility",
        "The best way to type better is to type more",
        "Typing is both a skill and an art",
        "Celebrate small improvements each session",
        "Typing well helps daily digital activities",
        "Strong hands support independent living",
        "Confidence grows with steady progress",
        "Typing connects thought to action quickly",
        "Fine motor skills improve with careful typing",
        "Typing challenges keep the brain active",
        "Recovery through games makes progress fun",
        "Typing practice helps with real world tasks",
        "Every phrase typed is progress made",
        "Typing fluency makes tasks less frustrating",
        "Keyboard skills are useful at any age",
        "Typing supports memory and concentration",
        "Fast accurate typing saves time online",
        "Typing builds rhythm in thought and action",
        "Hand rehabilitation can be enjoyable",
        "Typing makes communication faster and easier",
        "Consistent training leads to steady progress",
        "Typing correctly prevents bad habits",
        "Typing fluency reduces hand strain",
        "Typing challenges are great brain workouts",
        "Accuracy today becomes speed tomorrow",
        "Typing is useful for writing and messaging",
        "Games make rehabilitation engaging",
        "Typing encourages focus and patience",
        "Practice regularly for lasting results",
        "Typing fluency supports everyday independence",
        "Typing helps improve reaction speed",
        "Correct typing form prevents mistakes",
        "Typing builds dexterity and control",
        "Typing trains both mind and body",
        "Fast typing comes with steady practice",
        "Typing can be both fun and therapeutic",
        "A clear mind leads to better typing",
        "Typing often improves everyday confidence",
        "Consistency is the key to improvement",
        "Typing helps with learning new skills",
        "Typing challenges the memory as well",
        "Typing fluency leads to less frustration",
        "Typing connects ideas to the screen quickly",
        "Typing fluency is rewarding to achieve",
        "Games make skill building enjoyable",
        "Typing practice sharpens focus",
        "Typing strengthens the small muscles in hands",
        "Typing challenges reaction and recall",
        "Improved typing supports digital literacy",
        "Typing fluency increases independence",
        "Typing builds confidence with technology",
        "Practice typing to improve accuracy",
        "Typing helps keep the mind engaged",
        "Typing fluency helps in daily communication",
        "Typing practice strengthens neural pathways",
        "Typing fluency supports better hand recovery",
        "Typing helps build lifelong digital skills",
        "Typing often can boost confidence",
        "Typing fluency is useful for all ages",
        "Typing exercises are good for the brain",
        "Typing fluency builds independence",
        "Typing strengthens memory and reflexes",
        "Typing is a valuable everyday ability"
    };

    [Header("Game Rules")]
    public int mistakeLimit = 10;   // max mistakes allowed before game ends
    public GameObject gameOverPanel; // assign a UI panel in Inspector

    private string currentPhrase;
    private HashSet<int> usedIndices = new HashSet<int>();
    private int correctKeystrokes = 0;
    private int totalKeystrokes = 0;
    private int mistakeCount = 0;

    private float startTime;
    private bool timerRunning = false;
    private bool gameOver = false;

    private int lastInputLength = 0;

    void Start()
    {
        LoadNextPhrase();
        inputField.onValueChanged.AddListener(OnInputChanged);

        int fontSize = PlayerPrefs.GetInt("FontSize", 48);
        phraseText.fontSize = fontSize;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void LoadNextPhrase()
    {
        if (phrases.Count == 0 || gameOver) return;

        if (usedIndices.Count == phrases.Count)
            usedIndices.Clear();

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, phrases.Count);
        } while (usedIndices.Contains(randomIndex));

        usedIndices.Add(randomIndex);
        currentPhrase = phrases[randomIndex];

        phraseText.text = currentPhrase;
        inputField.text = "";
        feedbackText.text = "";
        lastInputLength = 0;
    }

    void OnInputChanged(string userInput)
    {
        if (gameOver) return;

        if (!timerRunning)
        {
            timerRunning = true;
            startTime = Time.time;
        }

        if (userInput.Length > lastInputLength)
        {
            totalKeystrokes++;
        }

        if (userInput.Length > currentPhrase.Length)
        {
            AddMistake("<color=red>Too many characters!</color>");
            lastInputLength = userInput.Length;
            UpdateStats();
            return;
        }

        int firstErrorIndex = -1;
        for (int i = 0; i < userInput.Length; i++)
        {
            if (userInput[i] != currentPhrase[i] && firstErrorIndex == -1)
                firstErrorIndex = i;
        }

        string coloredPhrase;
        if (firstErrorIndex == -1)
        {
            coloredPhrase = $"<color=green>{currentPhrase.Substring(0, userInput.Length)}</color>" +
                            currentPhrase.Substring(userInput.Length);
            feedbackText.text = "<color=green>Correct so far...</color>";
        }
        else
        {
            coloredPhrase = $"<color=green>{currentPhrase.Substring(0, firstErrorIndex)}</color>" +
                            $"<color=red>{currentPhrase.Substring(firstErrorIndex, userInput.Length - firstErrorIndex)}</color>" +
                            currentPhrase.Substring(userInput.Length);

            feedbackText.text = "<color=red>Incorrect!</color>";

            if (userInput.Length > lastInputLength)
                AddMistake("<color=red>Incorrect!</color>");
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

    void AddMistake(string message)
    {
        mistakeCount++;
        feedbackText.text = message;
        if (mistakeCount >= mistakeLimit)
        {
            EndGame();
        }
    }

    void UpdateStats()
    {
        float elapsedMinutes = (Time.time - startTime) / 60f;
        int wordsTyped = correctKeystrokes / 5;
        int wpm = elapsedMinutes > 0 ? (int)(wordsTyped / elapsedMinutes) : 0;

        float accuracy = (totalKeystrokes > 0)
            ? (correctKeystrokes / (float)totalKeystrokes) * 100f
            : 0f;

        // Update WPM and Mistakes
        wpmText.text = $"WPM: {wpm}";
        mistakesText.text = $"Mistakes: {mistakeCount}/{mistakeLimit}";

        // Accuracy with color feedback
        accuracyText.text = "Accuracy: " + accuracy.ToString("F1") + "%";

        if (accuracy >= 90f)
        {
            accuracyText.color = Color.green;   // great accuracy
        }
        else if (accuracy >= 70f)
        {
            accuracyText.color = Color.yellow;  // decent accuracy
        }
        else
        {
            accuracyText.color = Color.red;     // needs improvement
        }
    }


    void EndGame()
    {
        gameOver = true;
        inputField.interactable = false;
        feedbackText.text = "<color=red>Game Over! Too many mistakes.</color>";

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}