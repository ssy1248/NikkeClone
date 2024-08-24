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
        //GameManager���� EnemyList �� ����  CollisitionEnter�� �ϸ� EnemyList�� �߰�
        gm = FindAnyObjectByType<GameManager>();

        if (gm == null)
        {
            Debug.LogError("GameManager�� ã�� �� �����ϴ�.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.transform.GetComponent<Enemy>();
        if (enemy != null)
        {
            gm.EnemyList.Add(enemy);
            Debug.Log("�浹�� �� �߰�: " + enemy.name);
        }
    }
}
