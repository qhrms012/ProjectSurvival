using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUIController : MonoBehaviour
{
    public GameObject leaderboardUI;  // 리더보드 UI 패널
    public Button leaderboardButton;  // 리더보드 버튼

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
    }

    private void UpdateButtonVisibility()
    {
        if (GameManager.Instance.isLive)
        {
            leaderboardButton.gameObject.SetActive(false); // 게임이 진행 중일 때 버튼 숨김
        }
        else
        {
            leaderboardButton.gameObject.SetActive(true);  // 게임이 끝났을 때 버튼 표시
        }
    }
}
