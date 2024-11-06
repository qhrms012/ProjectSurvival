using System.Collections;
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

        // �������� UI�� Ȱ��ȭ�Ǹ� �����͸� ���� ȣ��
        if (leaderboardUI.activeSelf)
        {
            StartCoroutine(LoadLeaderboardWithDelay());
        }
    }

    private IEnumerator LoadLeaderboardWithDelay()
    {
        yield return new WaitForSeconds(0.1f); // 0.1�� ���� �� ȣ��
        leaderboard.LoadLeaderboardFromFirebase();
    }

    private void UpdateButtonVisibility()
    {
        // isLive ���¿� ���� ��ư ǥ��/�����
        leaderboardButton.gameObject.SetActive(!GameManager.Instance.isLive);
    }
}
