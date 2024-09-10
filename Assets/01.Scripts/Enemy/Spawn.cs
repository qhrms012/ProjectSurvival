using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Spawn : MonoBehaviour
{

    public Transform player;
    float spawnTimer;
    int level;
    [Header("SpawnDistance")]
    public float spwanRadius = 10f;
    public float minSpawnDistance = 5f;




    private void Update()
    {
        spawnTimer += Time.deltaTime;
        level = Mathf.FloorToInt(GameManager.Instance.gameTime / 10f);
        if(spawnTimer > (level == 0 ? 0.5f : 0.2f))
        {
            spawnTimer = 0;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        // 랜덤한 각도를 계산
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // 스폰 위치 계산 (랜덤 방향으로 반경 내에서)
        float spawnDistance = Random.Range(minSpawnDistance, spwanRadius);
        Vector3 spawnPosition = player.position + new Vector3(randomDirection.x, randomDirection.y, randomDirection.y) * spawnDistance;

        // 몹을 해당 위치에 소환
        GameObject enemy = GameManager.Instance.ObjectPool.Get(level);
        enemy.transform.position = spawnPosition;
    }
}
