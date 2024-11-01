using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Firebase.Auth;

public class AuthManager : MonoBehaviour
{
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] GameObject authNotice;

    private FirebaseAuth auth;
    private WaitForSecondsRealtime wait;

    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        wait = new WaitForSecondsRealtime(2);
    }

    public async void Login()
    {
        try
        {
            await auth.SignInWithEmailAndPasswordAsync(emailField.text, passwordField.text);
            Debug.Log(emailField.text + " 로 로그인 하셨습니다.");
        }
        catch
        {
            // 로그인 실패 시 코루틴 호출
            StartCoroutine(ShowPopupRoutine(0)); // 0번 자식 (로그인 실패 팝업)
        }
    }

    public async void Register()
    {
        try
        {
            await auth.CreateUserWithEmailAndPasswordAsync(emailField.text, passwordField.text);
            Debug.Log(emailField.text + "로 회원가입 성공");
        }
        catch
        {
            // 회원가입 실패 시 코루틴 호출
            StartCoroutine(ShowPopupRoutine(1)); // 1번 자식 (회원가입 실패 팝업)
        }
    }

    IEnumerator ShowPopupRoutine(int childIndex)
    {
        // 적절한 팝업만 표시
        authNotice.SetActive(true);
        for (int index = 0; index < authNotice.transform.childCount; index++)
        {
            authNotice.transform.GetChild(index).gameObject.SetActive(index == childIndex);
        }

        yield return wait;

        authNotice.SetActive(false);
    }
}
