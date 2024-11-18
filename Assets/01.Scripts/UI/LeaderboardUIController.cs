using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUIController : MonoBehaviour
{
    public GameObject leaderboardUI;    // �������� UI �г�
    public Button leaderboardButton;    // �������� ��ư
    public LeaderBoard leaderboard;     // LeaderBoard ��ũ��Ʈ ����

    private void Start()
    {
        // �ʱ� UI ���� ����
        leaderboardUI.SetActive(false);
        UpdateButtonVisibility();
    }

    private void Update()
    {
        // GameManager�� isLive ���¿� ���� ��ư ǥ�� ���� ������Ʈ
        UpdateButtonVisibility();
    }

    public void ToggleLeaderboard()
    {
        // �������� UI�� Ȱ��ȭ ���¸� ���
        leaderboardUI.SetActive(!leaderboardUI.activeSelf);

        if (leaderboardUI.activeSelf)
        {
            LoadLeaderboardWithDelay();
        }
    }

    public async void LoadLeaderboardWithDelay()
    {

        // �������� �����͸� �ҷ���
        List<Tuple<string, float, int, Sprite>> leaderboardData = await DatabaseManager.Instance.LoadLeaderboardEntries();

        if (leaderboardData != null)
        {   
            // �����Ͱ� �����ϸ� �� �׸��� ���������� AddEntry ȣ��
            foreach (var entry in leaderboardData)
            {
                leaderboard.AddEntry(entry.Item1, entry.Item2, entry.Item3, entry.Item4);
            }

            // UI ����
            leaderboard.UpdateLeaderboardUI();
        }
    }
    // �̸��Ϸ� ���͵� �������� �ҷ����� �޼���
    public async void LoadUserLeaderboard()
    {
        string userEmail = FirebaseAuth.DefaultInstance.CurrentUser.Email;
        List<Tuple<string, float, int, Sprite>> userLeaderboardData = await DatabaseManager.Instance.LoadUserLeaderboardEntries(userEmail);

        if (userLeaderboardData != null)
        {
            leaderboard.ClearEntries();
            foreach (var entry in userLeaderboardData)
            {
                leaderboard.AddEntry(entry.Item1, entry.Item2, entry.Item3, entry.Item4);
            }
            leaderboard.UpdateLeaderboardUI();
        }
    }
    private void UpdateButtonVisibility()
    {
        // isLive ���¿� ���� ��ư ǥ��/�����
        leaderboardButton.gameObject.SetActive(!GameManager.Instance.isLive);
    }
}
