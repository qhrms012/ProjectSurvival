using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passwordField;

    Firebase.Auth.FirebaseAuth auth;

    private void Awake()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(emailField.text, passwordField.text)
            .ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log(emailField.text + " 로 로그인 하셨습니다.");
            }
            else
            {
                Debug.Log("로그인 실패");
            }
        }
        );
    }

    public void Register()
    {
        auth.CreateUserWithEmailAndPasswordAsync(emailField.text, passwordField.text)
            .ContinueWith(task =>
            {
                if(!task.IsCanceled && !task.IsFaulted)
                {
                    Debug.Log(emailField.text + "로 회원가입\n");
                }
                else
                {
                    Debug.Log("회원가입 실패\n");
                }
            }
            );
    }
}
