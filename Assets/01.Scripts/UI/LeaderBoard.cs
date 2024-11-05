using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    private List<Tuple<string, float, int, Sprite>> leaderboard = new List<Tuple<string, float, int, Sprite>>();

    public Image[] characterImages;  // UI에 표시할 캐릭터 스프라이트를 위한 Image 배열
    public TextMeshProUGUI[] remainingTimeTextUI; // 남은 시간을 표시할 Text 배열
    public TextMeshProUGUI[] killCountTextUI; // 킬 수를 표시할 Text 배열


    void SortLeaderboard()
    {
        leaderboard = leaderboard.OrderBy(record => record.Item2).ToList();
    }

    public void AddToLeaderboard(string playerName, float remainingTime)
    {
        int killCount = GameManager.Instance.kill;
        Sprite characterSprite = GameManager.Instance.player.GetCharacterSprite(); // 고정된 스프라이트 가져오기
        leaderboard.Add(new Tuple<string, float, int, Sprite>(playerName, remainingTime, killCount, characterSprite));
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

