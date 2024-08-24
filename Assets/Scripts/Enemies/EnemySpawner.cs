using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //Enemy 스폰 위치 (50, 100, 150) 3군데
    public List<Transform> enemySpawnPoints;
    //Enemy Prefab 넣을 리스트
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
            AliveEnemy++; // 스폰한 적 수 증가
            Timer = 3f;
        }
    }

    public void CheckEnemy()
    {
        // 현재 씬에 있는 모든 적을 찾아 AliveEnemy 수를 업데이트
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        AliveEnemy = enemies.Length; // 현재 살아있는 적의 수를 업데이트
    }

    bool SpawnTimer()
    {
        Timer -= Time.deltaTime; // 시간 감소

        if (Timer <= 0)
        {
            return true;
        }
        else
            return false;
    }
}