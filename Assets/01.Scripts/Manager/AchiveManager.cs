using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject uiNotice;
    enum Achive { UnlockFrog, UnlockMask, UnlockMaskHead, UnlockHiddenMask}
    Achive[] achives;

    WaitForSecondsRealtime wait;

    private void Awake()
    {
        achives = (Achive[])Enum.GetValues(typeof(Achive));
        wait = new WaitForSecondsRealtime(5);
        //if(!PlayerPrefs.HasKey("MyData"))
            //Init();
    }

    //void Init()
    //{
    //    PlayerPrefs.SetInt("MyData" , 1);

    //    foreach (Achive achive in achives)
    //    {
    //        PlayerPrefs.SetInt(achive.ToString(), 0);
    //    }
    //}
    // Start is called before the first frame update
    //void Start()
    //{
    //    UnlockCharacter();
    //}
    async void Start()
    {
        string userId = DatabaseManager.Instance.GetUserId();

        if (!string.IsNullOrEmpty(userId))
        {
            // ���� ID�� ������ ���� �����͸� �ε�
            Dictionary<string, int> achievements = await DatabaseManager.Instance.LoadAchievements(userId);
            if (achievements != null)
            {
                foreach (var achive in achives)
                {
                    string achiveName = achive.ToString();
                    int value = achievements.ContainsKey(achiveName) ? achievements[achiveName] : 0;
                    PlayerPrefs.SetInt(achiveName, value);
                }
            }
            else
            {
                // ���� �����Ͱ� ���� ��� �⺻������ ����
                Debug.LogWarning("No achievements found for user. Setting default values.");
                foreach (var achive in achives)
                {
                    string achiveName = achive.ToString();
                    PlayerPrefs.SetInt(achiveName, 0); // �⺻�� 0���� ���� (��� ���� ����)
                }
            }
        }
        else
        {
            Debug.LogWarning("User ID is null or empty.");
        }

        // �׻� ĳ���� ��� ���� �۾��� ����
        UnlockCharacter();
    }

    void UnlockCharacter()
    {
        for (int index = 0; index < lockCharacter.Length; index++)
        {
            string achiveName = achives[index].ToString();
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1;
            lockCharacter[index].SetActive(!isUnlock);
            unlockCharacter[index].SetActive(isUnlock);
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        foreach(Achive achive in achives)
        {
            CheckAchive(achive);
        }
    }
    async void CheckAchive(Achive achive)
    {
        bool isAchive = false;

        switch (achive)
        {
            case Achive.UnlockFrog:
                isAchive = GameManager.Instance.kill >= 200;
                break;
            case Achive.UnlockMask:
                isAchive = GameManager.Instance.gameTime >= 120f;
                break;
            case Achive.UnlockMaskHead:
                isAchive = GameManager.Instance.kill >= 2000;
                break;
            case Achive.UnlockHiddenMask:
                isAchive = GameManager.Instance.gameTime == GameManager.Instance.maxGameTime;
                break;
        }

        if (isAchive && PlayerPrefs.GetInt(achive.ToString()) == 0)
        {
            PlayerPrefs.SetInt(achive.ToString(), 1);

            string userId = DatabaseManager.Instance.GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                // 1. Firebase���� ���� ���� ������ �ҷ�����
                Dictionary<string, int> currentAchievements = await DatabaseManager.Instance.LoadAchievements(userId);
                if (currentAchievements == null)
                {
                    currentAchievements = new Dictionary<string, int>();
                }

                // 2. ���ο� ���� ������ ����
                currentAchievements[achive.ToString()] = 1;

                // 3. ���յ� �����͸� Firebase�� ����
                await DatabaseManager.Instance.SaveAchievements(userId, currentAchievements);
            }

            for (int index = 0; index < uiNotice.transform.childCount; index++)
            {
                bool isActive = index == (int)achive;
                uiNotice.transform.GetChild(index).gameObject.SetActive(isActive);
            }

            StartCoroutine(NoticeRoutine());
        }
    }


    //void CheckAchive(Achive achive)
    //{

    //    //bool isAchive = false;


    //    //switch (achive)
    //    //{
    //    //    case Achive.UnlockFrog:
    //    //        isAchive = GameManager.Instance.kill >= 10;
    //    //        break;

    //    //    case Achive.UnlockMask:
    //    //        isAchive = GameManager.Instance.gameTime == GameManager.Instance.maxGameTime;
    //    //        break;
    //    //}
    //    //if (isAchive &&PlayerPrefs.GetInt(achive.ToString()) == 0)
    //    //{
    //    //    PlayerPrefs.SetInt(achive.ToString(), 1);

    //    //    for(int index = 0; index < uiNotice.transform.childCount; index++)
    //    //    {
    //    //        bool isActive = index == (int)achive;
    //    //        uiNotice.transform.GetChild(index).gameObject.SetActive(isActive);
    //    //    }

    //    //    StartCoroutine(NoticeRoutine());
    //    //}
    //}

    IEnumerator NoticeRoutine()
    {

        uiNotice.SetActive(true);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.LevelUp);

        yield return wait;

        uiNotice.SetActive(false);


    }
}
