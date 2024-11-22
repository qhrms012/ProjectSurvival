using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using System;
using Firebase.Analytics;

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
    private bool isFirebaseInitialized = false;
    private async void Awake()
    {
        Debug.Log("AuthManager Awake ȣ��");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("�ߺ��� AuthManager �ı�");
            Destroy(gameObject);
            return;
        }
        wait = new WaitForSecondsRealtime(2);

        InitializeFirebase();
    }
    private void InitializeFirebase()
    {
        Debug.Log("InitializeFirebase �޼��� ȣ���");
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            Debug.Log("Firebase ������ Ȯ�� �۾� �Ϸ�"); // �۾� �Ϸ� �� �׻� ���
            if (task.IsFaulted)
            {
                Debug.LogError($"Firebase ������ Ȯ�� �� ���� �߻�: {task.Exception}");
                return;
            }
            var dependencyStatus = task.Result;
            Debug.Log($"Firebase ������ ����: {dependencyStatus}"); // ������ ���� �α� �߰�

            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Firebase�� ���������� �ʱ�ȭ��
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                auth = FirebaseAuth.DefaultInstance;
                isFirebaseInitialized = true;
                Debug.Log("Firebase �ʱ�ȭ ����");
                // auth �ʱ�ȭ Ȯ�� �α� �߰�
                if (auth == null)
                {
                    Debug.LogError("FirebaseAuth�� �ʱ�ȭ���� �ʾҽ��ϴ�.");
                }
                else
                {
                    Debug.Log("FirebaseAuth �ʱ�ȭ ����");
                }
            }
            else
            {
                // Firebase �ʱ�ȭ ����
                Debug.LogError($"Firebase �ʱ�ȭ ����: {dependencyStatus}");
            }
        });
    }
    public async void Login()
    {
        if (!isFirebaseInitialized)
        {
            Debug.LogError("Firebase �ʱ�ȭ�� �Ϸ���� �ʾҽ��ϴ�. ��� �� �ٽ� �õ��� �ּ���.");
            return;
        }
        Debug.Log("Login �޼��� ȣ��");
        if (emailField == null || passwordField == null)
        {
            Debug.LogError("emailField �Ǵ� passwordField�� null�Դϴ�.");
            return;
        }

        Debug.Log($"�Էµ� �̸���: {emailField.text}, �Էµ� ��й�ȣ: {passwordField.text}");

        if (string.IsNullOrWhiteSpace(emailField.text) || string.IsNullOrWhiteSpace(passwordField.text))
        {
            Debug.LogError("�̸��� �Ǵ� ��й�ȣ�� ��� �ֽ��ϴ�.");
            ShowPopup(0); // ���� �˾�
            return;
        }

        if (auth == null)
        {
            Debug.LogError("FirebaseAuth�� �ʱ�ȭ���� �ʾҽ��ϴ�.");
            return;
        }

        try
        {
            Debug.Log("Firebase �α��� ��û ��...");
            var userCredential = await auth.SignInWithEmailAndPasswordAsync(emailField.text, passwordField.text);

            var user = userCredential.User;
            Debug.Log($"{emailField.text} �� �α��� ����");

            UserId = user.UserId;
            SceneManager.LoadScene("Mainscene");
        }
        catch (Exception e)
        {
            Debug.LogError($"�α��� ����: {e.Message}");
            ShowPopup(0); // ���� �˾�
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
