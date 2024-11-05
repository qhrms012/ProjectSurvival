using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUIController : MonoBehaviour
{
    public GameObject leaderboardUI;    // �������� UI �г�
    public Button leaderboardButton;    // �������� ��ư

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

        // UI�� ���� ���� ��, �ٸ� ȭ���� Ŭ���ϸ� UI ��Ȱ��ȭ
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
        // �������� UI�� Ȱ��ȭ ���¸� ���
        leaderboardUI.SetActive(!leaderboardUI.activeSelf);
    }

    private void UpdateButtonVisibility()
    {
        // isLive ���¿� ���� ��ư ǥ��/�����
        leaderboardButton.gameObject.SetActive(!GameManager.Instance.isLive);
    }
}
