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
            Debug.Log(emailField.text + " �� �α��� �ϼ̽��ϴ�.");
        }
        catch
        {
            // �α��� ���� �� �ڷ�ƾ ȣ��
            StartCoroutine(ShowPopupRoutine(0)); // 0�� �ڽ� (�α��� ���� �˾�)
        }
    }

    public async void Register()
    {
        try
        {
            await auth.CreateUserWithEmailAndPasswordAsync(emailField.text, passwordField.text);
            Debug.Log(emailField.text + "�� ȸ������ ����");
        }
        catch
        {
            // ȸ������ ���� �� �ڷ�ƾ ȣ��
            StartCoroutine(ShowPopupRoutine(1)); // 1�� �ڽ� (ȸ������ ���� �˾�)
        }
    }

    IEnumerator ShowPopupRoutine(int childIndex)
    {
        // ������ �˾��� ǥ��
        authNotice.SetActive(true);
        for (int index = 0; index < authNotice.transform.childCount; index++)
        {
            authNotice.transform.GetChild(index).gameObject.SetActive(index == childIndex);
        }

        yield return wait;

        authNotice.SetActive(false);
    }
}
