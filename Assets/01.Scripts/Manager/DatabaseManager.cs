using Firebase.Auth;
using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine;

public class DatabaseManager : Singleton<DatabaseManager>
{
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;

    private void Awake()
    {

        DontDestroyOnLoad(gameObject);


        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public string GetUserId()
    {
        if (auth.CurrentUser != null)
        {
            return auth.CurrentUser.UserId;
        }
        Debug.LogWarning("User is not authenticated.");
        return null;
    }

    public Task SaveLeaderboardEntry(string userId, string userEmail, float remainingTime, int killCount)
    {
        var leaderboardEntry = new Dictionary<string, object>
    {
        { "userEmail", userEmail },
        { "remainingTime", remainingTime },
        { "killCount", killCount }
    };

        return databaseReference.Child("leaderboard").Child(userId).Push().SetValueAsync(leaderboardEntry);
    }


    // �������� �����͸� Firebase���� �ҷ����� �޼���
    public async Task<List<Tuple<string, float, int, Sprite>>> LoadLeaderboardEntries()
    {
        if (databaseReference == null)
        {
            Debug.LogError("Database reference is null. Make sure InitializeFirebase() is called.");
            return null;
        }

        DataSnapshot snapshot;
        try
        {
            snapshot = await databaseReference.Child("leaderboard").GetValueAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to load leaderboard entries: " + ex.Message);
            return null;
        }

        List<Tuple<string, float, int, Sprite>> leaderboardData = new List<Tuple<string, float, int, Sprite>>();

        foreach (var userSnapshot in snapshot.Children)
        {
            foreach (var entrySnapshot in userSnapshot.Children)
            {
                // userEmail �����ϰ� �б�
                string userEmail = entrySnapshot.Child("userEmail").Value != null
                    ? entrySnapshot.Child("userEmail").Value.ToString()
                    : "Unknown Player"; // �⺻ �̸� ����

                // remainingTime�� �����ϰ� �б�
                float remainingTime = entrySnapshot.Child("remainingTime").Value != null
                    ? float.Parse(entrySnapshot.Child("remainingTime").Value.ToString())
                    : 0f; // �⺻ �ð� ����

                // killCount�� �����ϰ� �б�
                int killCount = entrySnapshot.Child("killCount").Value != null
                    ? int.Parse(entrySnapshot.Child("killCount").Value.ToString())
                    : 0; // �⺻ ų �� ����

                // characterSprite ó��
                Sprite characterSprite = null;
                string spritePath = "CharacterSprites/DefaultSprite"; // �⺻ ��������Ʈ ����

                var characterSpriteValue = entrySnapshot.Child("characterSprite").Value;
                if (characterSpriteValue != null && characterSpriteValue.ToString() != "")
                {
                    spritePath = "CharacterSprites/" + characterSpriteValue.ToString();
                }

                characterSprite = Resources.Load<Sprite>(spritePath);
                if (characterSprite == null)
                {
                    Debug.LogWarning($"Character sprite not found at path: {spritePath}");
                }

                leaderboardData.Add(new Tuple<string, float, int, Sprite>(userEmail, remainingTime, killCount, characterSprite));
            }
        }


        return leaderboardData;
    }
}
