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
                Debug.Log(emailField.text + " �� �α��� �ϼ̽��ϴ�.");
            }
            else
            {
                Debug.Log("�α��� ����");
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
                    Debug.Log(emailField.text + "�� ȸ������\n");
                }
                else
                {
                    Debug.Log("ȸ������ ����\n");
                }
            }
            );
    }
}
