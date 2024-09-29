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

    private bool isReloading; // 리로드 상태를 추적하는 변수
    private LineRenderer lineRenderer; // 라인 렌더러 변수 추가

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        ui = FindObjectOfType<UIManager>();
        player = gameObject.GetComponent<PlayerStat>();

        player.PlayerIndex = gm.PlayerObjects.IndexOf(gameObject);

        isAlive = true;

        fireCooldown = 0f;

        lineSetting();
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

    private void lineSetting()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // 혹시 모를 null 체크
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer가 이 오브젝트에 존재하지 않습니다!");
        }
        else
        {
            // 기본 설정 (필요한 경우)
            lineRenderer.positionCount = 2; // 시작점과 끝점
            lineRenderer.enabled = false;   // 처음에는 비활성화 상태
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

            if (player.currentWeapon == Weapons.SG)
            {
                Debug.Log("샷건 장전");
            }

            // 코루틴을 시작하여 리로드를 처리
            StartCoroutine(RealoadTimeCheck(player.currentReloadTime));

            Debug.Log("탄창 0발");
            return;
        }

        player.currentAmmo -= 1;
        ui.ammoUIManager.UpdateAutoAttackAmmoUI(player);

        target.TakeDamage(player.currentDamage + TypeChart.GetEffectiveness(player.currentCodes, target.enemyStat.currentCodes) - target.enemyStat.currentDefece);

        // 공격 시 라인 렌더러를 사용하여 시각적 효과 추가
        StartCoroutine(ShowAttackLine(target));

        if (ui.burstManager.BurstIndex == 0)
        {
            ui.burstManager.UpdateBurstBar(player.currentDamage + TypeChart.GetEffectiveness(player.currentCodes, target.enemyStat.currentCodes));
        }

        fireCooldown = 1f; // 공격 후 쿨타임 설정

        // 적이 죽었는지 확인
        if (!target.isAlive)
        {
            gm.EnemyList.Remove(target);
        }
    }

    public IEnumerator RealoadTimeCheck(float ReloadTime)
    {
        isReloading = true; // 리로드 시작 상태 설정

        while (ReloadTime > 0)
        {
            ReloadTime -= Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 리로드가 완료되었을 때만 실제 리로드 처리
        if (isReloading)
        {
            Reload(); // 리로드 완료 후 탄창 갱신
        }

        isReloading = false; // 리로드 상태 해제
    }

    // 공격 시 라인 렌더러를 잠시 활성화하는 코루틴
    IEnumerator ShowAttackLine(Enemy target)
    {
        lineRenderer.enabled = true; // 라인 렌더러 활성화
        lineRenderer.SetPosition(0, transform.position); // 시작점: 플레이어 위치
        lineRenderer.SetPosition(1, target.transform.position); // 끝점: 적의 위치

        yield return new WaitForSeconds(0.3f); // 0.1초 동안 선을 표시

        lineRenderer.enabled = false; // 라인 렌더러 비활성화
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
