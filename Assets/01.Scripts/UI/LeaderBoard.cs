using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Storage;

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
            // Firebase에서 리더보드 항목 로드 (Clear 제거)
            leaderboard = await DatabaseManager.Instance.LoadLeaderboardEntries();

            // UI 갱신
            UpdateLeaderboardUI();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to load leaderboard entries: {e.Message}");
        }
    }


    // 리더보드에 항목을 추가하고 Firebase에 저장
    public async void AddToLeaderboard(float remainingTime)
    {
        int killCount = GameManager.Instance.kill;
        Sprite characterSprite = GameManager.Instance.player.GetCharacterSprite();

        string userId = DatabaseManager.Instance.GetUserId();
        string userEmail = FirebaseAuth.DefaultInstance.CurrentUser.Email;
        //string characterSpriteUrl = FirebaseStorage.DefaultInstance.ToString();

        // 캐릭터 이미지를 Firebase에 업로드하고 URL을 가져옴
        string characterImageUrl = await DatabaseManager.Instance.UploadFirstFrameToStorage(characterSprite, userId);
        if (userId != null)
        {
            try
            {
                await DatabaseManager.Instance.SaveLeaderboardEntry(userId, userEmail, remainingTime, killCount, characterImageUrl);
                Debug.Log("Leaderboard entry saved successfully.");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to save leaderboard entry: {e.Message}");
            }
        }

        leaderboard.Clear();
        // UI 갱신
        leaderboard.Add(new Tuple<string, float, int, Sprite>(userEmail, remainingTime, killCount, characterSprite));
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
    public void AddEntry(string userEmail, float remainingTime, int killCount, Sprite characterSprite)
    {
        // 중복 항목 확인 후 추가 - 같은 playerName과 remainingTime을 가진 항목이 이미 있는지 확인
        bool isDuplicate = leaderboard.Any(entry =>
            entry.Item1 == userEmail &&
            Mathf.Approximately(entry.Item2, remainingTime) &&
            entry.Item3 == killCount);

        if (!isDuplicate)
        {
            // 중복이 아닌 경우에만 항목 추가
            leaderboard.Add(new Tuple<string, float, int, Sprite>(userEmail, remainingTime, killCount, characterSprite));
        }

        // UI 갱신
        UpdateLeaderboardUI();
    }

    // 리더보드 초기화
    public void ClearEntries()
    {
        leaderboard.Clear();
        UpdateLeaderboardUI();
    }
}
