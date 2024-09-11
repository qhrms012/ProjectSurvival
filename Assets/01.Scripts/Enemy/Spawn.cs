using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Spawn : MonoBehaviour
{

    public Transform player;
    public SpawnData[] spawnData;

    float spawnTimer;
    int level;
    [Header("SpawnDistance")]
    public float spwanRadius = 10f;
    public float minSpawnDistance = 5f;




    private void Update()
    {
        spawnTimer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.Instance.gameTime / 10f), spawnData.Length -1);
        if(spawnTimer > spawnData[level].spawnTime)
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
        Vector3 spawnPosition = player.position + new Vector3(randomDirection.x, randomDirection.y, 0) * spawnDistance;

        // ���� �ش� ��ġ�� ��ȯ
        GameObject enemy = GameManager.Instance.ObjectPool.Get(0);
        enemy.transform.position = spawnPosition;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
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
