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
            // SignInWithEmailAndPasswordAsync는 UserCredential 객체를 반환
            var userCredential = await auth.SignInWithEmailAndPasswordAsync(emailField.text, passwordField.text);

            // 로그인한 사용자의 UserId는 UserCredential 객체에서 user 속성으로 접근 가능
            var user = userCredential.User;
            Debug.Log(emailField.text + " 로 로그인 하셨습니다.");

            // 로그인 후 UserId 저장
            UserId = user.UserId;

            // 메인 씬으로 전환
            SceneManager.LoadScene("Mainscene");
        }
        catch (Exception e)
        {
            Debug.LogError("로그인 실패: " + e.Message);
            ShowPopup(0); // 로그인 실패 팝업
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
