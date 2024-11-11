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


    // 리더보드 데이터를 Firebase에서 불러오는 메서드
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
                // userEmail 안전하게 읽기
                string userEmail = entrySnapshot.Child("userEmail").Value != null
                    ? entrySnapshot.Child("userEmail").Value.ToString()
                    : "Unknown Player"; // 기본 이름 설정

                // remainingTime을 안전하게 읽기
                float remainingTime = entrySnapshot.Child("remainingTime").Value != null
                    ? float.Parse(entrySnapshot.Child("remainingTime").Value.ToString())
                    : 0f; // 기본 시간 설정

                // killCount를 안전하게 읽기
                int killCount = entrySnapshot.Child("killCount").Value != null
                    ? int.Parse(entrySnapshot.Child("killCount").Value.ToString())
                    : 0; // 기본 킬 수 설정

                // characterSprite 처리
                Sprite characterSprite = null;
                string spritePath = "CharacterSprites/DefaultSprite"; // 기본 스프라이트 설정

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
