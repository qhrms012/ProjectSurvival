using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Spawn : MonoBehaviour
{

    public Transform player;
    public SpawnData[] spawnData;
    public float levelTime;

    float spawnTimer;
    int level;
    [Header("SpawnDistance")]
    public float spwanRadius = 10f;
    public float minSpawnDistance = 5f;


    private void Awake()
    {
        levelTime = GameManager.Instance.maxGameTime / spawnData.Length;
    }

    private void Update()
    {
        if (!GameManager.Instance.isLive)
            return;

        spawnTimer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.Instance.gameTime / levelTime), spawnData.Length - 1);
        if (spawnTimer > spawnData[level].spawnTime)
        {
            spawnTimer = 0;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (PhotonNetwork.IsMasterClient) // ������ Ŭ���̾�Ʈ������ ��ġ ���
        {
            // ������ ������ ���
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // ���� ��ġ ��� (���� �������� �ݰ� ������)
            float spawnDistance = Random.Range(minSpawnDistance, spwanRadius);
            Vector3 spawnPosition = player.position + new Vector3(randomDirection.x, randomDirection.y, 0) * spawnDistance;

            // RPC ȣ��� ��� Ŭ���̾�Ʈ�� ��ġ ����
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("SpawnEnemyAtPosition", RpcTarget.All, spawnPosition, level);
        }
    }
    [PunRPC]
    void SpawnEnemyAtPosition(Vector3 spawnPosition, int spawnLevel)
    {
        // ��� Ŭ���̾�Ʈ���� ������ ��ġ�� ���� ����
        GameObject enemy = GameManager.Instance.objectPool.Get(0);
        enemy.transform.position = spawnPosition;
        enemy.GetComponent<Enemy>().Init(spawnData[spawnLevel]);
    }
}



[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
}
