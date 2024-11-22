using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using System;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }
    public string UserId { get; private set; }

    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] GameObject authNotice;

    private FirebaseAuth auth;
    private WaitForSecondsRealtime wait;
    private bool isPopupActive = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        auth = FirebaseAuth.DefaultInstance;
        wait = new WaitForSecondsRealtime(2);
    }

    public async void Login()
    {
        try
        {
            // SignInWithEmailAndPasswordAsync�� UserCredential ��ü�� ��ȯ
            var userCredential = await auth.SignInWithEmailAndPasswordAsync(emailField.text, passwordField.text);

            // �α����� ������� UserId�� UserCredential ��ü���� user �Ӽ����� ���� ����
            var user = userCredential.User;
            Debug.Log(emailField.text + " �� �α��� �ϼ̽��ϴ�.");

            // �α��� �� UserId ����
            UserId = user.UserId;

            // ���� ������ ��ȯ
            SceneManager.LoadScene("Mainscene");
        }
        catch (Exception e)
        {
            Debug.LogError("�α��� ����: " + e.Message);
            ShowPopup(0); // �α��� ���� �˾�
        }
    }

    public async void Register()
    {
        try
        {
            await auth.CreateUserWithEmailAndPasswordAsync(emailField.text, passwordField.text);
            ShowPopup(2);
        }
        catch
        {
            // ȸ������ ���� �� �ڷ�ƾ ȣ��
            ShowPopup(1); // 1�� �ڽ� (ȸ������ ���� �˾�)
        }
    }

    public async void ShowPopup(int childIndex)
    {
        if (isPopupActive) return;

        isPopupActive = true;

        authNotice.SetActive(true);
        for (int index = 0; index < authNotice.transform.childCount; index++)
        {
            authNotice.transform.GetChild(index).gameObject.SetActive(index == childIndex);
        }

        await Task.Delay(2000); // 2�� ���

        authNotice.SetActive(false);
        isPopupActive = false;
    }
}
