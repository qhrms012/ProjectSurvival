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



    public TextMeshProUGUI[] remainingTimeTextUI; // ���� �ð��� ǥ���� Text �迭
    public TextMeshProUGUI[] killCountTextUI; // ų ���� ǥ���� Text �迭

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

    // ���� �� UI ������Ʈ
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
            }
            else
            {
                remainingTimeTextUI[i].text = ""; // �����Ͱ� ������ ��ĭ
                killCountTextUI[i].text = ""; // �����Ͱ� ������ ��ĭ
            }
        }
    }



}
