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

    public Task SaveLeaderboardEntry(string userId, string playerName, float remainingTime, int killCount)
    {
        var leaderboardEntry = new Dictionary<string, object>
        {
            { "playerName", playerName },
            { "remainingTime", remainingTime },
            { "killCount", killCount }
        };

        return databaseReference.Child("leaderboard").Child(userId).Push().SetValueAsync(leaderboardEntry);
    }

    // 리더보드 데이터를 Firebase에서 불러오는 메서드
    public async Task<List<Tuple<string, float, int, Sprite>>> LoadLeaderboardEntries()
    {
        DataSnapshot snapshot = await databaseReference.Child("leaderboard").GetValueAsync();
        List<Tuple<string, float, int, Sprite>> leaderboardData = new List<Tuple<string, float, int, Sprite>>();

        foreach (var userSnapshot in snapshot.Children)
        {
            foreach (var entrySnapshot in userSnapshot.Children)
            {
                string playerName = entrySnapshot.Child("playerName").Value.ToString();
                float remainingTime = float.Parse(entrySnapshot.Child("remainingTime").Value.ToString());
                int killCount = int.Parse(entrySnapshot.Child("killCount").Value.ToString());

                // characterSprite를 안전하게 읽기
                Sprite characterSprite = null;
                string spritePath = "";

                var characterSpriteValue = entrySnapshot.Child("characterSprite").Value;
                if (characterSpriteValue != null && characterSpriteValue.ToString() != "")
                {
                    spritePath = "CharacterSprites/" + characterSpriteValue.ToString();
                }
                else
                {
                    Debug.LogWarning("No character sprite found for player: " + playerName);
                    spritePath = "CharacterSprites/DefaultSprite"; // 기본 스프라이트로 대체
                }

                try
                {
                    characterSprite = Resources.Load<Sprite>(spritePath);
                    if (characterSprite == null)
                    {
                        Debug.LogWarning("Character sprite not found at path: " + spritePath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Error loading sprite at path: " + spritePath + " - " + ex.Message);
                }

                leaderboardData.Add(new Tuple<string, float, int, Sprite>(playerName, remainingTime, killCount, characterSprite));
            }
        }

        return leaderboardData;
    }
}
