using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUIController : MonoBehaviour
{
    public GameObject leaderboardUI;  // �������� UI �г�
    public Button leaderboardButton;  // �������� ��ư

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
    }

    private void UpdateButtonVisibility()
    {
        if (GameManager.Instance.isLive)
        {
            leaderboardButton.gameObject.SetActive(false); // ������ ���� ���� �� ��ư ����
        }
        else
        {
            leaderboardButton.gameObject.SetActive(true);  // ������ ������ �� ��ư ǥ��
        }
    }
}
