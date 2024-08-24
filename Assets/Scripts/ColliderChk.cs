using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColliderChk : MonoBehaviour
{
    GameManager gm;
    AutoPlayer autoP;

    private void Start()
    {
        //GameManager에서 EnemyList 를 만들어서  CollisitionEnter를 하면 EnemyList에 추가
        gm = FindAnyObjectByType<GameManager>();

        if (gm == null)
        {
            Debug.LogError("GameManager를 찾을 수 없습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.transform.GetComponent<Enemy>();
        if (enemy != null)
        {
            gm.EnemyList.Add(enemy);
            Debug.Log("충돌한 적 추가: " + enemy.name);
        }
    }
}
