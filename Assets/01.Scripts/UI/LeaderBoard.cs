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
        // Firebase���� �������� �׸� �ε�
        LoadLeaderboardFromFirebase();
    }

    // Firebase���� �������� �׸��� �ҷ����� �޼���
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

    // �������忡 �׸��� �߰��ϰ� Firebase�� ����
    public async void AddToLeaderboard(string playerName, float remainingTime)
    {
        int killCount = GameManager.Instance.kill;
        Sprite characterSprite = GameManager.Instance.player.GetCharacterSprite();

        // Firebase�� ����
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

        // UI ���� (����Ʈ�� �׸��� �߰��ϰ� �� ���� UI�� ������Ʈ)
        leaderboard.Add(new Tuple<string, float, int, Sprite>(playerName, remainingTime, killCount, characterSprite));
        UpdateLeaderboardUI();
    }

    // UI ����
    public void UpdateLeaderboardUI()
    {
        // �ð��� ���� ���� (��������)
        var sortedLeaderboard = leaderboard.OrderBy(record => record.Item2).ToList();

        // UI ������Ʈ
        for (int i = 0; i < remainingTimeTextUI.Length; i++)
        {
            if (i < sortedLeaderboard.Count)
            {
                remainingTimeTextUI[i].text = $"{i + 1}. {sortedLeaderboard[i].Item1}: {sortedLeaderboard[i].Item2:F2} ��";
                killCountTextUI[i].text = $"{sortedLeaderboard[i].Item3} ų";
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

    // �������� �׸��� �߰��ϰ� UI�� ������Ʈ
    public void AddEntry(string playerName, float remainingTime, int killCount, Sprite characterSprite)
    {
        leaderboard.Add(new Tuple<string, float, int, Sprite>(playerName, remainingTime, killCount, characterSprite));

        // UI ����
        UpdateLeaderboardUI();
    }
}
