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
    public float spawnRadius = 10f;
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
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트에서만 위치 계산
        {
            // 랜덤한 각도를 계산
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // 스폰 위치 계산 (랜덤 방향으로 반경 내에서)
            float spawnDistance = Random.Range(minSpawnDistance, spawnRadius);
            Vector3 spawnPosition = player.position + new Vector3(randomDirection.x, randomDirection.y, 0) * spawnDistance;

            // 모든 클라이언트에 적을 생성하는 RPC 호출
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("SyncEnemyProperties", RpcTarget.AllBuffered, spawnPosition, level); // Buffered로 모든 클라이언트에 동일 상태 유지
        }
    }

    [PunRPC]
    void SyncEnemyProperties(Vector3 spawnPosition, int spawnLevel)
    {
        // PhotonNetwork.Instantiate로 네트워크에서 관리되는 적 생성
        GameObject enemy = PhotonNetwork.Instantiate("Enemy", spawnPosition, Quaternion.identity);
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
