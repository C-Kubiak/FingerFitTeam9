using System.IO;
using UnityEngine;

public static class UserDataManager
{
    private static string folderPath = Path.Combine(Application.dataPath, "UserData");

    public static void SaveUserData(UserData user)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string fileName = $"{user.userName}_{user.birthday}.json";
        string filePath = Path.Combine(folderPath, fileName);

        string json = JsonUtility.ToJson(user, true);
        File.WriteAllText(filePath, json);

        Debug.Log("Saved data for " + user.userName + " at " + filePath);
    }

    public static UserData LoadUserData(string name, string birthday)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string fileName = $"{name}_{birthday}.json";
        string filePath = Path.Combine(folderPath, fileName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            UserData user = JsonUtility.FromJson<UserData>(json);
            Debug.Log("Loaded data for " + name + " from " + filePath);
            return user;
        }
        else
        {
            UserData newUser = new UserData(name, birthday);
            SaveUserData(newUser);
            Debug.Log("Created new data for " + name + " at " + filePath);
            return newUser;
        }
    }
}
