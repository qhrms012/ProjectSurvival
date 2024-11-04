using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class LeaderBoard : MonoBehaviour
{
    private List<Tuple<string, float, int>> leaderboard = new List<Tuple<string, float, int>>();



    public TextMeshProUGUI[] remainingTimeTextUI; // 남은 시간을 표시할 Text 배열
    public TextMeshProUGUI[] killCountTextUI; // 킬 수를 표시할 Text 배열

    void SortLeaderboard()
    {
        leaderboard = leaderboard.OrderBy(record => record.Item2).ToList();
    }

    public void AddToLeaderboard(string playerName, float remainingTime)
    {
        int killCount = GameManager.Instance.kill;
        leaderboard.Add(new Tuple<string, float, int>(playerName, remainingTime, killCount));
        UpdateLeaderboardUI();
    }

    // 정렬 및 UI 업데이트
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
            }
            else
            {
                remainingTimeTextUI[i].text = ""; // 데이터가 없으면 빈칸
                killCountTextUI[i].text = ""; // 데이터가 없으면 빈칸
            }
        }
    }



}
