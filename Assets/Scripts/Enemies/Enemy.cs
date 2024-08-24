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

    //��������, ������ �����ؼ� �̵�
    //�̵��߿� ����? �ƴ� �̵��� �÷��̾� ���� ��Ÿ� ������ ����?

    //ex) ������ -> �߾����� ���� �������� ��Ƽ� �����ϸ� ����
    //ex) ������ -> ���� ��ġ�� ���� ���������� ��Ƽ� �����ϸ� ����
    //ex) ����� -> ���������� ���� ���������� ������ �����ð��� �� ��� �ΰ� ���� ��Ÿ���� �Ǹ� ����?

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
