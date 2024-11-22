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
        Debug.Log("AuthManager Awake 호출");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("중복된 AuthManager 파괴");
            Destroy(gameObject);
            return;
        }
        wait = new WaitForSecondsRealtime(2);

        InitializeFirebase();
    }
    private void InitializeFirebase()
    {
        Debug.Log("InitializeFirebase 메서드 호출됨");
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            Debug.Log("Firebase 의존성 확인 작업 완료"); // 작업 완료 시 항상 출력
            if (task.IsFaulted)
            {
                Debug.LogError($"Firebase 의존성 확인 중 오류 발생: {task.Exception}");
                return;
            }
            var dependencyStatus = task.Result;
            Debug.Log($"Firebase 의존성 상태: {dependencyStatus}"); // 의존성 상태 로그 추가

            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Firebase가 정상적으로 초기화됨
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                auth = FirebaseAuth.DefaultInstance;
                isFirebaseInitialized = true;
                Debug.Log("Firebase 초기화 성공");
                // auth 초기화 확인 로그 추가
                if (auth == null)
                {
                    Debug.LogError("FirebaseAuth가 초기화되지 않았습니다.");
                }
                else
                {
                    Debug.Log("FirebaseAuth 초기화 성공");
                }
            }
            else
            {
                // Firebase 초기화 실패
                Debug.LogError($"Firebase 초기화 실패: {dependencyStatus}");
            }
        });
    }
    public async void Login()
    {
        if (!isFirebaseInitialized)
        {
            Debug.LogError("Firebase 초기화가 완료되지 않았습니다. 잠시 후 다시 시도해 주세요.");
            return;
        }
        Debug.Log("Login 메서드 호출");
        if (emailField == null || passwordField == null)
        {
            Debug.LogError("emailField 또는 passwordField가 null입니다.");
            return;
        }

        Debug.Log($"입력된 이메일: {emailField.text}, 입력된 비밀번호: {passwordField.text}");

        if (string.IsNullOrWhiteSpace(emailField.text) || string.IsNullOrWhiteSpace(passwordField.text))
        {
            Debug.LogError("이메일 또는 비밀번호가 비어 있습니다.");
            ShowPopup(0); // 실패 팝업
            return;
        }

        if (auth == null)
        {
            Debug.LogError("FirebaseAuth가 초기화되지 않았습니다.");
            return;
        }

        try
        {
            Debug.Log("Firebase 로그인 요청 중...");
            var userCredential = await auth.SignInWithEmailAndPasswordAsync(emailField.text, passwordField.text);

            var user = userCredential.User;
            Debug.Log($"{emailField.text} 로 로그인 성공");

            UserId = user.UserId;
            SceneManager.LoadScene("Mainscene");
        }
        catch (Exception e)
        {
            Debug.LogError($"로그인 실패: {e.Message}");
            ShowPopup(0); // 실패 팝업
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
            // 회원가입 실패 시 코루틴 호출
            ShowPopup(1); // 1번 자식 (회원가입 실패 팝업)
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

        await Task.Delay(2000); // 2초 대기

        authNotice.SetActive(false);
        isPopupActive = false;
    }
}
