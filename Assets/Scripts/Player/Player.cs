using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject Scope;
    public float rayDistance = 1000f; // 레이의 거리
    GameManager gm;

    PlayerStat player;
    Enemy enemy;
    UIManager ui;

    public Image AmmoBar;
    public Image HpBar;
    public TextMeshProUGUI AmmoText;
    public bool isAlive;

    private float fireCooldown;

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        ui = FindObjectOfType<UIManager>();
        player = gameObject.GetComponent<PlayerStat>();

        if (Scope == null)
        {
            Debug.LogError("Scope 오브젝트가 할당되지 않았습니다.");
        }
        else
        {
            Scope.SetActive(false);
        }

        isAlive = true;

        fireCooldown = 0f;
    }

    void Update()
    {
        if (enabled)
        {
            MouseToRotatePosition();

            // 발사 속도 재설정
            if (fireCooldown >= 0)
            {
                fireCooldown -= Time.deltaTime;
            }
        }
    }

    void MouseToRotatePosition()
    {
        // 마우스 이동 시 좌표 가져오기
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        // 마우스 클릭 중일 때 Scope 오브젝트 위치 업데이트
        if (Input.GetMouseButton(0)) // 왼쪽 마우스 버튼 클릭 중일 때
        {
            AmmoUIUpdate();
            Scope.SetActive(true);
            Scope.transform.position = mousePosition;

            // Raycast로 적 오브젝트 감지
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    Scope.transform.position = hit.point; // Scope를 적 위치로 이동
                    if (fireCooldown <= 0)
                    {
                        Attack(hit.transform.GetComponent<Enemy>());
                        fireCooldown = player.currentRateOfFire; // 발사 속도 초기화
                    }
                }
                else if(hit.transform.CompareTag("Breakable"))
                {
                    //건물
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Scope.SetActive(false);
            Reload();
        }
    }

    public void Attack(Enemy targetEnemy)
    {
        if (player.currentAmmo <= 0)
        {
            //임의 체크
            player.currentAmmo = 0;
            Debug.Log("탄창 0발");
            return;
        }

        player.currentAmmo -= 1;
        ui.ammoUIManager.UpdateAttckAmmoUI(player);

        if (targetEnemy != null)
        {
            enemy = targetEnemy;

            Debug.Log(player.currentDamage + TypeChart.GetEffectiveness(player.currentCodes, enemy.enemyStat.currentCodes) - enemy.enemyStat.currentDefece);
            if(ui.burstManager.BurstIndex == 0)
            {
                ui.burstManager.UpdateBurstBar(player.currentDamage + TypeChart.GetEffectiveness(player.currentCodes, enemy.enemyStat.currentCodes));
            }
           
            enemy.TakeDamage(player.currentDamage + TypeChart.GetEffectiveness(player.currentCodes, enemy.enemyStat.currentCodes) - enemy.enemyStat.currentDefece);
        }
    }

    public void TakeDamage(float dmg)
    {
        player.currentHealth -= dmg;

        if(player.currentHealth <= 0)
        {
            isAlive = false;
            Death();
        }
    }

    public void Death()
    {
        Destroy(this.gameObject);
    }

    //SG을 제외한 모든총기류에게 사용할 장전 함수
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

    void AmmoUIUpdate()
    {
        int Index = gm.PlayerIndex;
        float fill = player.currentAmmo / gm.MaxAmmos[Index];

        AmmoText.text = player.currentAmmo.ToString();
        AmmoBar.fillAmount = fill;
    }
}
