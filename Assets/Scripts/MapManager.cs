using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //�μ����� ������Ʈ�� ��ġ�� ���� ��ġ ����Ʈ
    public List<Transform> SpawnPoints;
    //�μ����� ������Ʈ �������� ������� ������Ʈ ����Ʈ
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
