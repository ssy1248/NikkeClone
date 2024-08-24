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
    private Enemy currentTarget; // 현재 타겟

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

            // 발사 속도 재설정
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
            // 랜덤 인덱스 선택
            int randomIndex = Random.Range(0, gm.EnemyList.Count);
            currentTarget = gm.EnemyList[randomIndex];


            Debug.Log("AutoAim Script 실행");

            // 공격
            if (fireCooldown <= 0)
            {
                // 현재 타겟이 살아있다면
                if (currentTarget.isAlive) 
                {
                    Attack(currentTarget);
                }
                // 현재 타겟이 죽었다면
                else
                {
                    // 재귀 호출로 랜덤 타겟 선택
                    AutoAim();
                }
            }
        }
    }

    void Attack(Enemy target)
    {
        if (player.currentAmmo <= 0)
        {
            //임의 체크
            player.currentAmmo = 0;
            Reload();
            Debug.Log("탄창 0발");
            return;
        }

        player.currentAmmo -= 1;
        ui.ammoUIManager.UpdateAutoAttackAmmoUI(player);

        target.TakeDamage(player.currentDamage);
        fireCooldown = 1f; // 공격 후 쿨타임 설정

        // 적이 죽었는지 확인
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
        // SG 전용 장전 함수
    }
}
