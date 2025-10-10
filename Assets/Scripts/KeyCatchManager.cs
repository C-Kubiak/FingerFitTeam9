using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KeyCatchManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text keyPromptText;    // big letter prompt
    public TMP_Text scoreText;        // score counter
    public TMP_Text timerText;        // timer
    public TMP_Text reactionText;     // shows avg reaction time
    public TMP_InputField inputField; // input box
    public Button playAgainButton;    // play again button

    [Header("Game Rules")]
    public float gameDuration = 45f;
    public string[] possibleKeys = {
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
        "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
    };

    private string currentKey;
    private int score = 0;
    private float timeRemaining;
    private bool gameActive = false;

    private float keyShownTime;  // when the key appeared

    // Reaction tracking
    private float totalReactionTime = 0f;
    private int successfulReactions = 0;

    void Start()
    {
        playAgainButton.onClick.AddListener(StartGame);

        inputField.onValueChanged.AddListener(OnInputChanged);

        inputField.characterLimit = 1; // only one char allowed
        StartGame();
    }

    void Update()
    {
        if (!gameActive) return;

        // countdown timer
        timeRemaining -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);

        if (timeRemaining <= 0f)
        {
            gameActive = false;
            keyPromptText.text = "";
            reactionText.text = $"Final Avg Reaction: {(successfulReactions > 0 ? (totalReactionTime / successfulReactions).ToString("F2") : "N/A")}s";
        }
    }

    void StartGame()
    {
        score = 0;
        totalReactionTime = 0f;
        successfulReactions = 0;

        scoreText.text = "Score: 0";
        reactionText.text = "Average Reaction: 0.00s";

        timeRemaining = gameDuration;
        gameActive = true;

        inputField.text = "";
        inputField.ActivateInputField();
        ShowNewKey();
    }

    private string lastKey = null; // store previous key

    void ShowNewKey()
    {
        string newKey;

        // keep picking until it's different from last key
        do
        {
            int randomIndex = Random.Range(0, possibleKeys.Length);
            newKey = possibleKeys[randomIndex];
        }
        while (newKey == lastKey);

        currentKey = newKey;
        lastKey = newKey; // update memory

        keyPromptText.text = currentKey;
        keyShownTime = Time.time; // reset reaction timer

        inputField.text = "";
        inputField.ActivateInputField();
    }


    void OnInputChanged(string userInput)
    {
        if (!gameActive) return;
        if (string.IsNullOrEmpty(userInput)) return;

        string typed = userInput.ToUpper();

        if (typed == currentKey)
        {
            // reaction time
            float reaction = Time.time - keyShownTime;
            totalReactionTime += reaction;
            successfulReactions++;

            float avgReaction = totalReactionTime / successfulReactions;
            reactionText.text = $"Average Reaction: {avgReaction:F2}s";

            // score
            score++;
            scoreText.text = "Score: " + score;

            ShowNewKey();
        }

        // always clear after checking
        inputField.text = "";
        inputField.ActivateInputField();
    }
}
