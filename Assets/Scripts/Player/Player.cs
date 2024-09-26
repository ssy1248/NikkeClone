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

    public Image RealoadBar;

    public Image AmmoBar;
    public Image HpBar;
    public TextMeshProUGUI AmmoText;
    public bool isAlive;

    private float fireCooldown;
    private bool isReloading; // 리로드 상태를 추적하는 변수

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
        isReloading = false; // 초기 상태는 리로드 중이 아님
    }

    void Update()
    {
        if (enabled)
        {
            if(!IsMouseOverUI())
            {
                MouseToRotatePosition();
            }


            // 발사 속도 재설정
            if (fireCooldown >= 0)
            {
                fireCooldown -= Time.deltaTime;
            }
        }
    }

    // 마우스가 UI 위에 있는지 확인하는 메서드
    private bool IsMouseOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    void MouseToRotatePosition()
    {
        // 마우스 이동 시 좌표 가져오기
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        // 마우스 위치로부터 앞으로 레이캐스트를 쏴서 적을 감지
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);

        // 마우스 클릭 중일 때 Scope 오브젝트 위치 업데이트
        if (Input.GetMouseButton(0) && !IsMouseOverUI()) // 왼쪽 마우스 버튼 클릭 중일 때
        {
            if (isReloading) // 리로드 중인 경우 코루틴을 중단하고 상태 업데이트
            {
                StopCoroutine("RealoadTimeCheck");
                isReloading = false;
                RealoadBar.gameObject.SetActive(false);
            }

            AmmoUIUpdate();
            Scope.SetActive(true);
            Scope.transform.position = mousePosition;

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    Scope.SetActive(true);
                    Scope.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y, hit.transform.position.z - 0.5f); // 스코프를 적 위치로 이동

                    if (fireCooldown <= 0)
                    {
                        Attack(hit.transform.GetComponent<Enemy>());
                        fireCooldown = player.currentRateOfFire; // 발사 속도 초기화
                    }

                    return; // 가장 가까운 적만 공격하고 나머지는 무시
                }
                else if (hit.transform.CompareTag("Breakable"))
                {

                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && !IsMouseOverUI())
        {
            Scope.SetActive(false);
            RealoadBar.gameObject.SetActive(true);

            // 코루틴을 시작하여 리로드를 처리
            StartCoroutine(RealoadTimeCheck(player.currentReloadTime));
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

        Debug.Log("공격 중");

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

    public void AutoAttack()
    {

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

    public IEnumerator RealoadTimeCheck(float ReloadTime)
    {
        isReloading = true; // 리로드 시작 상태 설정

        Image reloadFillImage = RealoadBar.transform.GetChild(0).GetComponent<Image>();
        reloadFillImage.fillAmount = 0; // 리로드 시작 시 초기화

        while (ReloadTime > 0)
        {
            ReloadTime -= Time.deltaTime;
            reloadFillImage.fillAmount = 1 - (ReloadTime / player.currentReloadTime); // 리로드 진행률 업데이트
            yield return null; // 다음 프레임까지 대기
        }

        // 리로드가 완료되었을 때만 실제 리로드 처리
        if (isReloading)
        {
            reloadFillImage.fillAmount = 1; // 리로드 완료
            Reload(); // 리로드 완료 후 탄창 갱신
            RealoadBar.gameObject.SetActive(false);
        }

        isReloading = false; // 리로드 상태 해제
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
