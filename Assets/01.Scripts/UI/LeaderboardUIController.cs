using System.Collections;
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

        // UI가 켜져 있을 때, 다른 화면을 클릭하면 UI 비활성화
        if (leaderboardUI.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                leaderboardUI.GetComponent<RectTransform>(),
                Input.mousePosition,
                Camera.main))
            {
                leaderboardUI.SetActive(false);
            }
        }
    }

    public void ToggleLeaderboard()
    {
        // 리더보드 UI의 활성화 상태를 토글
        leaderboardUI.SetActive(!leaderboardUI.activeSelf);

        // 리더보드 UI가 활성화되면 데이터를 지연 호출
        if (leaderboardUI.activeSelf)
        {
            StartCoroutine(LoadLeaderboardWithDelay());
        }
    }

    private IEnumerator LoadLeaderboardWithDelay()
    {
        yield return new WaitForSeconds(0.1f); // 0.1초 지연 후 호출
        leaderboard.LoadLeaderboardFromFirebase();
    }

    private void UpdateButtonVisibility()
    {
        // isLive 상태에 따라 버튼 표시/숨기기
        leaderboardButton.gameObject.SetActive(!GameManager.Instance.isLive);
    }
}
