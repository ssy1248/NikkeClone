using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //Enemy ���� ��ġ (50, 100, 150) 3����
    public List<Transform> enemySpawnPoints;
    //Enemy Prefab ���� ����Ʈ
    public List<GameObject> enemyPrefabs;
    public GameObject enemyPrefab;

    public int EnemyMaxSpawnCount = 10;
    public int EnemySpawnCount = 0;

    public float Timer;

    public int AliveEnemy = 0;

    void Start()
    {
        Timer = 3f;
    }

    void Update()
    {
        CheckEnemy();
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        CheckEnemy();

        if (AliveEnemy <= EnemyMaxSpawnCount && SpawnTimer())
        {
            int point = Random.Range(0, enemySpawnPoints.Count);
            GameObject enemy = Instantiate(enemyPrefab, enemySpawnPoints[point].position, enemySpawnPoints[point].rotation);
            enemy.transform.parent = enemySpawnPoints[point];
            enemy.GetComponent<Enemy>().startPoint = enemySpawnPoints[point].transform;

            EnemySpawnCount++;
            AliveEnemy++; // ������ �� �� ����
            Timer = 3f;
        }
    }

    public void CheckEnemy()
    {
        // ���� ���� �ִ� ��� ���� ã�� AliveEnemy ���� ������Ʈ
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        AliveEnemy = enemies.Length; // ���� ����ִ� ���� ���� ������Ʈ
    }

    bool SpawnTimer()
    {
        Timer -= Time.deltaTime; // �ð� ����

        if (Timer <= 0)
        {
            return true;
        }
        else
            return false;
    }
}