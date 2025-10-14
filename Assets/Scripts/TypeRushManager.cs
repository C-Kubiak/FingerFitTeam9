using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class TypeRushManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text phraseText;
    public TMP_InputField inputField;
    public TMP_Text wpmText;
    public TMP_Text accuracyText;
    public TMP_Text mistakesText;
    public TMP_Text timerText;
    public Button startButton;
    public Button playAgainButton;

    [Header("Phrases")]
    [SerializeField, TextArea(2, 5)]
    private List<string> phrases = new List<string>() {
        "the quick brown fox jumps over the lazy dog",
        "typing games help improve hand recovery",
        "unity makes game development accessible",
        "fingerfit is designed for older adults",
        "practice makes perfect when typing every day",
        "small consistent steps lead to big results",
        "healthy hands mean more freedom in daily life",
        "good posture helps with comfortable typing",
        "never give up on learning new skills",
        "patience and practice improve coordination",
        "typing trains both the brain and the hands",
        "every letter typed builds muscle memory",
        "speed comes naturally after accuracy",
        "focus on progress, not perfection",
        "typing is a useful everyday skill",
        "stay calm and keep practicing typing",
        "the journey is just as important as the goal",
        "typing exercises help strengthen fingers",
        "balance speed with accuracy for success",
        "typing with rhythm can improve flow",
        "always look ahead to the next challenge",
        "practice typing with joy and patience",
        "train your hands like an athlete trains muscles",
        "typing improves both memory and focus",
        "recovery takes time but effort pays off",
        "each keystroke brings you closer to mastery",
        "stay positive and consistent with training",
        "typing is a gateway to digital communication",
        "fast fingers make computer use easier",
        "strength grows when effort is repeated",
        "every challenge is a chance to grow",
        "practice typing in short focused sessions",
        "typing strengthens hand-eye coordination",
        "repetition builds familiarity and skill",
        "accuracy first, speed second, mastery third",
        "typing helps sharpen mental agility",
        "the best way to type better is to type more",
        "typing is both a skill and an art",
        "celebrate small improvements each session",
        "typing well helps daily digital activities",
        "strong hands support independent living",
        "confidence grows with steady progress",
        "typing connects thought to action quickly",
        "fine motor skills improve with careful typing",
        "typing challenges keep the brain active",
        "recovery through games makes progress fun",
        "typing practice helps with real world tasks",
        "every phrase typed is progress made",
        "typing fluency makes tasks less frustrating",
        "keyboard skills are useful at any age",
        "typing supports memory and concentration",
        "fast accurate typing saves time online",
        "typing builds rhythm in thought and action",
        "hand rehabilitation can be enjoyable",
        "typing makes communication faster and easier",
        "consistent training leads to steady progress",
        "typing correctly prevents bad habits",
        "typing fluency reduces hand strain",
        "typing challenges are great brain workouts",
        "accuracy today becomes speed tomorrow",
        "typing is useful for writing and messaging",
        "games make rehabilitation engaging",
        "typing encourages focus and patience",
        "practice regularly for lasting results",
        "typing fluency supports everyday independence",
        "typing helps improve reaction speed",
        "correct typing form prevents mistakes",
        "typing builds dexterity and control",
        "typing trains both mind and body",
        "fast typing comes with steady practice",
        "typing can be both fun and therapeutic",
        "a clear mind leads to better typing",
        "typing often improves everyday confidence",
        "consistency is the key to improvement",
        "typing helps with learning new skills",
        "typing challenges the memory as well",
        "typing fluency leads to less frustration",
        "typing connects ideas to the screen quickly",
        "typing fluency is rewarding to achieve",
        "games make skill building enjoyable",
        "typing practice sharpens focus",
        "typing strengthens the small muscles in hands",
        "typing challenges reaction and recall",
        "improved typing supports digital literacy",
        "typing fluency increases independence",
        "typing builds confidence with technology",
        "practice typing to improve accuracy",
        "typing helps keep the mind engaged",
        "typing fluency helps in daily communication",
        "typing practice strengthens neural pathways",
        "typing fluency supports better hand recovery",
        "typing helps build lifelong digital skills",
        "typing often can boost confidence",
        "typing fluency is useful for all ages",
        "typing exercises are good for the brain",
        "typing fluency builds independence",
        "typing strengthens memory and reflexes",
        "typing is a valuable everyday ability"
    };

    [Header("Game Rules")]
    public int mistakeLimit = 10;

    private string currentPhrase;
    private HashSet<int> usedIndices = new HashSet<int>();
    private int correctKeystrokes = 0;
    private int totalKeystrokes = 0;
    private int mistakeCount = 0;

    private float startTime;
    private bool timerRunning = false;
    private bool gameOver = false;
    private float elapsedTime = 0f;

    private int lastInputLength = 0;
    private int lastCorrectKeystrokes = 0;

    void Start()
    {
        // Setup listeners
        inputField.onValueChanged.AddListener(OnInputChanged);

        // Buttons
        if (startButton != null)
            startButton.onClick.AddListener(OnStartPressed);

        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(RestartGame);

        // UI setup
        int fontSize = PlayerPrefs.GetInt("FontSize", 48);
        phraseText.fontSize = fontSize;

        inputField.interactable = false; // wait for Start
        phraseText.text = "Press Start to Begin!";
        playAgainButton.gameObject.SetActive(false);
    }
    void Update()
    {
        if (timerRunning)
        {
            elapsedTime = Time.time - startTime;
            UpdateTimerText(elapsedTime);
        }
    }

    private void OnStartPressed()
    {
        startButton.gameObject.SetActive(false);
        BeginGame();
    }

    private void BeginGame()
    {
        gameOver = false;
        timerRunning = true;
        startTime = Time.time;
        correctKeystrokes = 0;
        totalKeystrokes = 0;
        mistakeCount = 0;
        usedIndices.Clear();

        playAgainButton.gameObject.SetActive(false);

        inputField.text = "";
        inputField.interactable = true;
        inputField.ActivateInputField();
        phraseText.text = "";
        lastInputLength = 0;
        lastCorrectKeystrokes = 0;

        LoadNextPhrase();
        UpdateStats();
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
        lastInputLength = 0;
        lastCorrectKeystrokes = 0;
    }

    void OnInputChanged(string userInput)
    {
        if (gameOver || !inputField.interactable) return;

        if (userInput.Length > lastInputLength)
            totalKeystrokes++;

        int correctThisInput = 0;
        for (int i = 0; i < userInput.Length; i++)
        {
            if (i < currentPhrase.Length && userInput[i] == currentPhrase[i])
                correctThisInput++;
        }

        correctKeystrokes += Mathf.Max(0, correctThisInput - lastCorrectKeystrokes);
        lastCorrectKeystrokes = correctThisInput;

        // Mistakes
        if (userInput.Length > currentPhrase.Length)
            AddMistake();

        // Highlight
        int firstErrorIndex = -1;
        for (int i = 0; i < userInput.Length; i++)
        {
            if (i >= currentPhrase.Length || userInput[i] != currentPhrase[i])
            {
                firstErrorIndex = i;
                break;
            }
        }

        string coloredPhrase;
        if (firstErrorIndex == -1)
        {
            coloredPhrase = $"<color=green>{currentPhrase.Substring(0, userInput.Length)}</color>" +
                            currentPhrase.Substring(userInput.Length);
        }
        else
        {
            coloredPhrase =
                $"<color=green>{currentPhrase.Substring(0, firstErrorIndex)}</color>" +
                $"<color=red>{currentPhrase.Substring(firstErrorIndex, Mathf.Min(userInput.Length, currentPhrase.Length) - firstErrorIndex)}</color>" +
                currentPhrase.Substring(Mathf.Min(userInput.Length, currentPhrase.Length));

            if (userInput.Length > lastInputLength)
                AddMistake();
        }

        phraseText.text = coloredPhrase;

        if (userInput == currentPhrase)
        {
            inputField.text = "";
            lastCorrectKeystrokes = 0;
            LoadNextPhrase();
        }

        lastInputLength = userInput.Length;
        UpdateStats();
    }

void AddMistake()
{
    mistakeCount++;
    UpdateStats();

    if (mistakeCount >= mistakeLimit)
    {
        phraseText.text = "<color=red><b>GAME OVER</b></color>";
        EndGame();
        return;
    }
}


    void UpdateStats()
    {
        float elapsedMinutes = timerRunning ? (Time.time - startTime) / 60f : 0f;
        int wordsTyped = correctKeystrokes / 5;
        int wpm = elapsedMinutes > 0 ? Mathf.RoundToInt(wordsTyped / elapsedMinutes) : 0;
        float accuracy = totalKeystrokes > 0
            ? (correctKeystrokes / (float)totalKeystrokes) * 100f
            : 100f;

        wpmText.text = $"WPM: {wpm}";
        mistakesText.text = $"Mistakes: {mistakeCount}/{mistakeLimit}";
        accuracyText.text = $"Accuracy: {accuracy:F1}%";

        if (accuracy >= 90f)
            accuracyText.color = Color.green;
        else if (accuracy >= 70f)
            accuracyText.color = Color.yellow;
        else
            accuracyText.color = Color.red;
    }

    void EndGame()
    {
        gameOver = true;
        timerRunning = false;
        inputField.interactable = false;
        playAgainButton.gameObject.SetActive(true);
    }

    void RestartGame()
    {
        startButton.gameObject.SetActive(true);
        playAgainButton.gameObject.SetActive(false);
        phraseText.text = "Press Start to Begin!";
        inputField.text = "";
        inputField.interactable = false;
        wpmText.text = "WPM: 0";
        accuracyText.text = "Accuracy: 100%";
        mistakesText.text = $"Mistakes: 0/{mistakeLimit}";
        elapsedTime = 0f;
        UpdateTimerText(0f);
    }

    private void UpdateTimerText(float t)
    {
        int minutes = (int)(t / 60f);
        int seconds = (int)(t % 60f);
        if (timerText != null)
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }
    
}