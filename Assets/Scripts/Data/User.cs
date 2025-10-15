using System;
using UnityEngine;

[Serializable]
public class UserData
{
    // --------------------------
    // Identification
    // --------------------------
    public string userName;
    public string birthday; // string format like "YYYY-MM-DD"

    // --------------------------
    // Global Stats
    // --------------------------
    public int totalGamesPlayed;
    public DateTime lastPlayed;

    // --------------------------
    // Mini-Game Specific Stats
    // --------------------------
    public int typeRushSessions;
    public float bestWPM;
    public float averageWPM;
    public float bestAccuracy;
    public float averageAccuracy;
    public int totalKeystrokes;
    public int totalMistakes;

    public int keyCatchSessions;
    public int bestKeyCatchScore;
    public float bestReactionTime;
    public float averageReactionTime;

    public int sequenceSparkSessions;
    public int bestSequenceScore;
    public int bestLongestStreak;
    public float averageMistakesPerGame;

    // --------------------------
    // Static Current User
    // --------------------------
    public static UserData CurrentUser; // globally accessible

    // --------------------------
    // Constructor
    // --------------------------
    public UserData(string name, string dob)
    {
        userName = name;
        birthday = dob;
        totalGamesPlayed = 0;
        lastPlayed = DateTime.Now;

        typeRushSessions = 0;
        bestWPM = 0;
        averageWPM = 0;
        bestAccuracy = 0;
        averageAccuracy = 0;
        totalKeystrokes = 0;
        totalMistakes = 0;

        keyCatchSessions = 0;
        bestKeyCatchScore = 0;
        bestReactionTime = 9999f;
        averageReactionTime = 0;

        sequenceSparkSessions = 0;
        bestSequenceScore = 0;
        bestLongestStreak = 0;
        averageMistakesPerGame = 0;
    }

    // 🟢 Load test user for testing
    public static void LoadTestUser()
    {
        CurrentUser = UserDataManager.LoadUserData("Test", "2000-01-01");
        Debug.Log("Loaded test user: " + CurrentUser.userName);
    }
}
