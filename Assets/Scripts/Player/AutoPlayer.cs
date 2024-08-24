using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoPlayer : MonoBehaviour
{
    GameManager gm;

    PlayerStat player;
    EnemyStat enemy;
    UIManager ui;

    public Image AmmoBar;
    public Image HpBar;
    public Text AmmoText;
    public bool isAlive;

    private float fireCooldown;
    private Enemy currentTarget; // ���� Ÿ��

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        ui = FindObjectOfType<UIManager>();
        player = gameObject.GetComponent<PlayerStat>();

        player.PlayerIndex = gm.PlayerObjects.IndexOf(gameObject);

        isAlive = true;

        fireCooldown = 0f;
    }

    void Update()
    {
        if (enabled)
        {
            AutoAim();

            // �߻� �ӵ� �缳��
            if (fireCooldown >= 0)
            {
                fireCooldown -= Time.deltaTime;
            }
        }
    }

    void AutoAim()
    { 
        if (gm.EnemyList.Count > 0)
        {
            // ���� �ε��� ����
            int randomIndex = Random.Range(0, gm.EnemyList.Count);
            currentTarget = gm.EnemyList[randomIndex];


            Debug.Log("AutoAim Script ����");

            // ����
            if (fireCooldown <= 0)
            {
                // ���� Ÿ���� ����ִٸ�
                if (currentTarget.isAlive) 
                {
                    Attack(currentTarget);
                }
                // ���� Ÿ���� �׾��ٸ�
                else
                {
                    // ��� ȣ��� ���� Ÿ�� ����
                    AutoAim();
                }
            }
        }
    }

    void Attack(Enemy target)
    {
        if (player.currentAmmo <= 0)
        {
            //���� üũ
            player.currentAmmo = 0;
            Reload();
            Debug.Log("źâ 0��");
            return;
        }

        player.currentAmmo -= 1;
        ui.ammoUIManager.UpdateAutoAttackAmmoUI(player);

        target.TakeDamage(player.currentDamage);
        fireCooldown = 1f; // ���� �� ��Ÿ�� ����

        // ���� �׾����� Ȯ��
        if (!target.isAlive)
        {
            gm.EnemyList.Remove(target);
        }
    }

    public void Reload()
    {
        player.currentAmmo = gm.MaxAmmos[gm.PlayerIndex];
        ui.ammoUIManager.UpdateReloadAmmoUI(player);
        AmmoBar.fillAmount = 1;
    }

    public void SGReload()
    {
        // SG ���� ���� �Լ�
    }
}
