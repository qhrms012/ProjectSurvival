using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;

    [Header("Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };


    [Header("Game Object")]
    public ObjectPool objectPool;
    public Player player;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public Transform uiJoyStick;
    public GameObject enemyCleaner;
    public LeaderBoard leaderBoard;

    private PhotonView pv;
    string playerName = "Player1";

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }


    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;

        // 기존에 활성화된 플레이어가 있다면 비활성화하거나 삭제
        if (player != null)
        {
            player.gameObject.SetActive(false); // 또는 Destroy(player.gameObject);
        }

        if (PhotonNetwork.IsConnected)
        {
            // 새로 생성된 플레이어 오브젝트를 네트워크에서 생성
            GameObject playerObj = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);

            // 새로 생성된 객체를 player에 할당
            player = playerObj.GetComponent<Player>();
        }

        // 새로 생성된 player 객체를 활성화 후 초기화 진행
        player.gameObject.SetActive(true);
        // Virtual Camera의 Follow 속성에 새로 생성된 플레이어의 Transform 할당
        var virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        virtualCamera.Follow = player.transform;   
        
        // UI 및 기타 초기화 설정
        uiLevelUp.Select(playerId % 2);
        Resume();

        AudioManager.Instance.PlayBgm(true);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);



        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();
        leaderBoard.AddToLeaderboard(gameTime);
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Lose);
        AudioManager.Instance.PlayBgm(false);
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();
        leaderBoard.AddToLeaderboard(gameTime);

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Win);
        AudioManager.Instance.PlayBgm(false);
    }


    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory();
        }     
    }
    public void GetExp()
    {
        if (!isLive)
            return;
            
        
        exp++;

        if (exp == nextExp[Mathf.Min(level, nextExp.Length -1)])
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
        uiJoyStick.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
        uiJoyStick.localScale = Vector3.one;
    }
}
