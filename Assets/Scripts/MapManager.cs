using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //부서지는 오브젝트를 설치할 스폰 위치 리스트
    public List<Transform> SpawnPoints;
    //부서지는 오브젝트 프리팹을 집어넣을 오브젝트 리스트
    public List<GameObject> BreakableObjects;

    private void Awake()
    {
        ObstacleCreate();
    }

    public void ObstacleCreate()
    {
        for(int i = 0; i < SpawnPoints.Count; i++)
        {
            GameObject obstacle = BreakableObjects[Random.Range(0, BreakableObjects.Count)];
            Transform spawnPoint = SpawnPoints[i];

            if (obstacle.name.Contains("LengthBreakableObject"))
            {
                spawnPoint.position = new Vector3(spawnPoint.position.x, 0.71f, spawnPoint.position.z);
            }
            else if (obstacle.name.Contains("WidthBreakableObject"))
            {
                spawnPoint.position = new Vector3(spawnPoint.position.x, 1.73f, spawnPoint.position.z);
            }

            Instantiate(obstacle, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
