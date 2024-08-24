using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    EnemySpawner spawner;

    public EnemyStat enemyStat;
    public Transform startPoint;
    public Transform endPoint;
    public Image enemyHpBar;

    public bool isAlive;

    //시작지점, 끝지점 생성해서 이동
    //이동중에 공격? 아님 이동중 플레이어 공격 사거리 잡히면 공격?

    //ex) 점령전 -> 중앙으로 엔드 포지션을 잡아서 도착하면 공격
    //ex) 보스전 -> 일정 위치를 엔드 포지션으로 잡아서 도착하면 공격
    //ex) 방어전 -> 도망지점을 엔드 포지션으로 잡지만 도착시간을 꽤 길게 두고 공격 쿨타임이 되면 공격?

    void Start()
    {
        isAlive = true;
        enemyStat = GetComponent<EnemyStat>();
        spawner = FindAnyObjectByType<EnemySpawner>();

        HpBarUpdate();
    }

    void Update()
    {
        if(enemyStat.currentHealth <= 0)
        {
            isAlive = false;
            Kill();
        }
    }

    void HpBarUpdate()
    {
        if (enemyHpBar != null)
        {
            enemyHpBar.fillAmount = enemyStat.currentHealth / enemyStat.MaxHealth;
        }
    }

    public void TakeDamage(float dmg)
    {
        enemyStat.currentHealth -= dmg;
        HpBarUpdate();

        if (enemyStat.currentHealth <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
