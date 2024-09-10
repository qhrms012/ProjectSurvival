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
        // ������ ������ ���
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // ���� ��ġ ��� (���� �������� �ݰ� ������)
        float spawnDistance = Random.Range(minSpawnDistance, spwanRadius);
        Vector3 spawnPosition = player.position + new Vector3(randomDirection.x, randomDirection.y, randomDirection.y) * spawnDistance;

        // ���� �ش� ��ġ�� ��ȯ
        GameObject enemy = GameManager.Instance.ObjectPool.Get(level);
        enemy.transform.position = spawnPosition;
    }
}
