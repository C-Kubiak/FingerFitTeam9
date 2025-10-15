using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField userNameInput;
    public TMP_InputField birthdayInput;
    public Button submitButton;
    public TMP_Text feedbackText;
    public GameObject loginPanel;

    private static bool loggedInThisSession = false; // track login in this play session

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmit);

        // Show login panel only if user hasn't logged in yet this session
        loginPanel.SetActive(!loggedInThisSession);

        feedbackText.text = "";
    }

    private void OnSubmit()
    {
        string name = userNameInput.text.Trim();
        string dob = birthdayInput.text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            feedbackText.text = "Please enter a name.";
            return;
        }

        if (!DateTime.TryParse(dob, out _))
        {
            feedbackText.text = "Invalid birthday format. Use YYYY-MM-DD";
            return;
        }

        // Load or create user
        UserData.CurrentUser = UserDataManager.LoadUserData(name, dob);

        feedbackText.text = $"Welcome, {UserData.CurrentUser.userName}!";

        // Hide login panel (optional, since we're changing scenes)
        loginPanel.SetActive(false);

        // ✅ Switch to HomePage scene
        SceneManager.LoadScene("HomePage");

        Debug.Log($"✅ Logged in as {UserData.CurrentUser.userName}");
    }

}
