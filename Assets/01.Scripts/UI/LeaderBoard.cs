using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    private List<Tuple<string, float, int, Sprite>> leaderboard = new List<Tuple<string, float, int, Sprite>>();

    public Image[] characterImages;
    public TextMeshProUGUI[] remainingTimeTextUI;
    public TextMeshProUGUI[] killCountTextUI;



    private void Start()
    {
        // Firebase에서 리더보드 항목 로드
        LoadLeaderboardFromFirebase();
    }

    // Firebase에서 리더보드 항목을 불러오는 메서드
    private async void LoadLeaderboardFromFirebase()
    {
        try
        {
            leaderboard = await DatabaseManager.Instance.LoadLeaderboardEntries();
            UpdateLeaderboardUI();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to load leaderboard entries: {e.Message}");
        }
    }

    // 리더보드에 항목을 추가하고 Firebase에 저장
    public async void AddToLeaderboard(string playerName, float remainingTime)
    {
        int killCount = GameManager.Instance.kill;
        Sprite characterSprite = GameManager.Instance.player.GetCharacterSprite();

        // Firebase에 저장
        string userId = DatabaseManager.Instance.GetUserId();
        if (userId != null)
        {
            try
            {
                await DatabaseManager.Instance.SaveLeaderboardEntry(userId, playerName, remainingTime, killCount);
                Debug.Log("Leaderboard entry saved successfully.");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to save leaderboard entry: {e.Message}");
            }
        }

        // UI 갱신 (리스트에 항목을 추가하고 한 번만 UI를 업데이트)
        leaderboard.Add(new Tuple<string, float, int, Sprite>(playerName, remainingTime, killCount, characterSprite));
        UpdateLeaderboardUI();
    }

    // UI 갱신
    public void UpdateLeaderboardUI()
    {
        // 시간에 따라 정렬 (오름차순)
        var sortedLeaderboard = leaderboard.OrderBy(record => record.Item2).ToList();

        // UI 업데이트
        for (int i = 0; i < remainingTimeTextUI.Length; i++)
        {
            if (i < sortedLeaderboard.Count)
            {
                remainingTimeTextUI[i].text = $"{i + 1}. {sortedLeaderboard[i].Item1}: {sortedLeaderboard[i].Item2:F2} 초";
                killCountTextUI[i].text = $"{sortedLeaderboard[i].Item3} 킬";
                characterImages[i].sprite = sortedLeaderboard[i].Item4;
                characterImages[i].gameObject.SetActive(true);
            }
            else
            {
                remainingTimeTextUI[i].text = "";
                killCountTextUI[i].text = "";
                characterImages[i].gameObject.SetActive(false);
            }
        }
    }

    // 리더보드 항목을 추가하고 UI를 업데이트
    public void AddEntry(string playerName, float remainingTime, int killCount, Sprite characterSprite)
    {
        leaderboard.Add(new Tuple<string, float, int, Sprite>(playerName, remainingTime, killCount, characterSprite));

        // UI 갱신
        UpdateLeaderboardUI();
    }
}
