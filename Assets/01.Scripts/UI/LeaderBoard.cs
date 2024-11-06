using Firebase.Database;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using System.Threading;

public class LeaderBoard : MonoBehaviour
{
    private List<Tuple<string, float, int, Sprite>> leaderboard = new List<Tuple<string, float, int, Sprite>>();

    public Image[] characterImages;  // UI�� ǥ���� ĳ���� ��������Ʈ�� ���� Image �迭
    public TextMeshProUGUI[] remainingTimeTextUI; // ���� �ð��� ǥ���� Text �迭
    public TextMeshProUGUI[] killCountTextUI; // ų ���� ǥ���� Text �迭

    private SynchronizationContext context;

    // Firebase �����ͺ��̽� ����
    private DatabaseReference databaseReference;

    private void Start()
    {
        // Firebase �����ͺ��̽� ���� �ʱ�ȭ
        databaseReference = FirebaseDatabase.DefaultInstance.GetReference("leaderboard");
        context = SynchronizationContext.Current;
    }

    public void AddToLeaderboard(string playerName, float remainingTime)
    {
        int killCount = GameManager.Instance.kill;
        Sprite characterSprite = GameManager.Instance.player.GetCharacterSprite(); // ĳ���� ��������Ʈ ��������
        leaderboard.Add(new Tuple<string, float, int, Sprite>(playerName, remainingTime, killCount, characterSprite));

        // Firebase�� ������ ����
        SaveLeaderboardEntryToFirebase(playerName, remainingTime, killCount);

        // UI ������Ʈ
        UpdateLeaderboardUI();
    }

    private void SaveLeaderboardEntryToFirebase(string playerName, float remainingTime, int killCount)
    {
        string key = databaseReference.Push().Key;  // Firebase���� ���� Ű ����
        var leaderboardEntry = new Dictionary<string, object>
        {
            { "playerName", playerName },
            { "remainingTime", remainingTime },
            { "killCount", killCount }
        };

        // Firebase�� ������ ����
        databaseReference.Child(key).SetValueAsync(leaderboardEntry).ContinueWith(task =>
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

                // ���� �����忡�� UI ������Ʈ
                context.Post(_ => UpdateLeaderboardUI(), null);
            }
            else
            {
                Debug.LogWarning("Failed to load leaderboard data.");
            }
        });
    }

    private void UpdateLeaderboardUI()
    {
        // ���� �ð��� �������� �������� ����
        var sortedLeaderboard = leaderboard.OrderBy(record => record.Item2).ToList();

        for (int i = 0; i < remainingTimeTextUI.Length; i++)
        {
            if (i < sortedLeaderboard.Count)
            {
                remainingTimeTextUI[i].text = $"{i + 1}. {sortedLeaderboard[i].Item1}: {sortedLeaderboard[i].Item2:F2} ��";
                killCountTextUI[i].text = $"{sortedLeaderboard[i].Item3} ų";

                // ĳ���� ��������Ʈ ����
                characterImages[i].sprite = sortedLeaderboard[i].Item4;
                characterImages[i].gameObject.SetActive(true);  // �̹����� ǥ�õǵ��� Ȱ��ȭ
            }
            else
            {
                remainingTimeTextUI[i].text = ""; // �����Ͱ� ������ ��ĭ
                killCountTextUI[i].text = ""; // �����Ͱ� ������ ��ĭ
                characterImages[i].gameObject.SetActive(false); // ������� �ʴ� �̹����� ��Ȱ��ȭ
            }
        }
    }
}
