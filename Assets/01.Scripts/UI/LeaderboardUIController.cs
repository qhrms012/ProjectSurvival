using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUIController : MonoBehaviour
{
    public GameObject leaderboardUI;    // 리더보드 UI 패널
    public Button leaderboardButton;    // 리더보드 버튼
    public LeaderBoard leaderboard;     // LeaderBoard 스크립트 참조

    private void Start()
    {
        // 초기 UI 상태 설정
        leaderboardUI.SetActive(false);
        UpdateButtonVisibility();
    }

    private void Update()
    {
        // GameManager의 isLive 상태에 따라 버튼 표시 여부 업데이트
        UpdateButtonVisibility();
    }

    public void ToggleLeaderboard()
    {
        // 리더보드 UI의 활성화 상태를 토글
        leaderboardUI.SetActive(!leaderboardUI.activeSelf);

        // 리더보드 UI가 활성화되면 데이터를 지연 호출
        if (leaderboardUI.activeSelf)
        {
            LoadLeaderboardWithDelay();
        }
    }

    private async void LoadLeaderboardWithDelay()
    {

        // 리더보드 데이터를 불러옴
        List<Tuple<string, float, int, Sprite>> leaderboardData = await DatabaseManager.Instance.LoadLeaderboardEntries();

        if (leaderboardData != null)
        {
            // 데이터가 존재하면 각 항목을 개별적으로 AddEntry 호출
            foreach (var entry in leaderboardData)
            {
                leaderboard.AddEntry(entry.Item1, entry.Item2, entry.Item3, entry.Item4);
            }

            // UI 갱신
            leaderboard.UpdateLeaderboardUI();
        }
    }

    private void UpdateButtonVisibility()
    {
        // isLive 상태에 따라 버튼 표시/숨기기
        leaderboardButton.gameObject.SetActive(!GameManager.Instance.isLive);
    }
}
