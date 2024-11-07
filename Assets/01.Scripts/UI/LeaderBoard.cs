using Firebase.Database;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using System.Threading;
using Firebase.Auth;

public class LeaderBoard : MonoBehaviour
{
    private List<Tuple<string, float, int, Sprite>> leaderboard = new List<Tuple<string, float, int, Sprite>>();

    public Image[] characterImages;  // UI에 표시할 캐릭터 스프라이트를 위한 Image 배열
    public TextMeshProUGUI[] remainingTimeTextUI; // 남은 시간을 표시할 Text 배열
    public TextMeshProUGUI[] killCountTextUI; // 킬 수를 표시할 Text 배열

    private SynchronizationContext context;

    // Firebase 데이터베이스 참조
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private string userId;

    private void Start()
    {
        // Firebase 데이터베이스 참조 초기화
        databaseReference = FirebaseDatabase.DefaultInstance.GetReference("leaderboard");
        context = SynchronizationContext.Current;

        auth = FirebaseAuth.DefaultInstance;
        userId = auth.CurrentUser.UserId;
    }

    public void AddToLeaderboard(string playerName, float remainingTime)
    {
        int killCount = GameManager.Instance.kill;
        Sprite characterSprite = GameManager.Instance.player.GetCharacterSprite(); // 캐릭터 스프라이트 가져오기
        leaderboard.Add(new Tuple<string, float, int, Sprite>(playerName, remainingTime, killCount, characterSprite));

        // Firebase에 데이터 저장
        SaveLeaderboardEntryToFirebase(playerName, remainingTime, killCount);

        // UI 업데이트
        UpdateLeaderboardUI();
    }

    private void SaveLeaderboardEntryToFirebase(string playerName, float remainingTime, int killCount)
    {
        var leaderboardEntry = new Dictionary<string, object>
        {
            { "playerName", playerName },
            { "remainingTime", remainingTime },
            { "killCount", killCount }
        };

        // Firebase에 데이터 저장
        databaseReference.Child(userId).SetValueAsync(leaderboardEntry).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Leaderboard entry saved successfully.");
            }
            else
            {
                Debug.LogWarning("Failed to save leaderboard entry.");
            }
        });
    }

    public async void LoadLeaderboardFromFirebase()
    {
        await databaseReference.GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted && task.Result.Exists)
            {
                leaderboard.Clear();

                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot playerData in snapshot.Children)
                {
                    string playerName = playerData.Child("playerName").Value.ToString();
                    float remainingTime = float.Parse(playerData.Child("remainingTime").Value.ToString());
                    int killCount = int.Parse(playerData.Child("killCount").Value.ToString());

                    Sprite characterSprite = GameManager.Instance.player.GetCharacterSprite();
                    leaderboard.Add(new Tuple<string, float, int, Sprite>(playerName, remainingTime, killCount, characterSprite));
                }

                // 메인 스레드에서 UI 업데이트
                context.Post(_ => UpdateLeaderboardUI(), null);
            }
            else
            {
                Debug.LogWarning("Failed to load leaderboard data.");
            }
        });
    }
    // 내 기록 불러오기
    public async void LoadPersonalRecord()
    {
        await databaseReference.Child(userId).GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted && task.Result.Exists)
            {
                leaderboard.Clear();

                DataSnapshot snapshot = task.Result;
                string playerName = snapshot.Child("playerName").Value.ToString();
                float remainingTime = float.Parse(snapshot.Child("remainingTime").Value.ToString());
                int killCount = int.Parse(snapshot.Child("killCount").Value.ToString());

                Sprite characterSprite = GameManager.Instance.player.GetCharacterSprite();
                leaderboard.Add(new Tuple<string, float, int, Sprite>(playerName, remainingTime, killCount, characterSprite));

                // 메인 스레드에서 UI 업데이트
                context.Post(_ => UpdateLeaderboardUI(), null);
            }
            else
            {
                Debug.LogWarning("No personal record found.");
            }
        });
    }

    private void UpdateLeaderboardUI()
    {
        // 남은 시간을 기준으로 오름차순 정렬
        var sortedLeaderboard = leaderboard.OrderBy(record => record.Item2).ToList();

        for (int i = 0; i < remainingTimeTextUI.Length; i++)
        {
            if (i < sortedLeaderboard.Count)
            {
                remainingTimeTextUI[i].text = $"{i + 1}. {sortedLeaderboard[i].Item1}: {sortedLeaderboard[i].Item2:F2} 초";
                killCountTextUI[i].text = $"{sortedLeaderboard[i].Item3} 킬";

                // 캐릭터 스프라이트 설정
                characterImages[i].sprite = sortedLeaderboard[i].Item4;
                characterImages[i].gameObject.SetActive(true);  // 이미지가 표시되도록 활성화
            }
            else
            {
                remainingTimeTextUI[i].text = ""; // 데이터가 없으면 빈칸
                killCountTextUI[i].text = ""; // 데이터가 없으면 빈칸
                characterImages[i].gameObject.SetActive(false); // 사용하지 않는 이미지는 비활성화
            }
        }
    }
}
