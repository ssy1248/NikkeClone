using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject Scope;
    public float rayDistance = 1000f; // ������ �Ÿ�
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
    private bool isReloading; // ���ε� ���¸� �����ϴ� ����

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        ui = FindObjectOfType<UIManager>();
        player = gameObject.GetComponent<PlayerStat>();

        if (Scope == null)
        {
            Debug.LogError("Scope ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�.");
        }
        else
        {
            Scope.SetActive(false);
        }

        isAlive = true;

        fireCooldown = 0f;
        isReloading = false; // �ʱ� ���´� ���ε� ���� �ƴ�
    }

    void Update()
    {
        if (enabled)
        {
            if(!IsMouseOverUI())
            {
                MouseToRotatePosition();
            }


            // �߻� �ӵ� �缳��
            if (fireCooldown >= 0)
            {
                fireCooldown -= Time.deltaTime;
            }
        }
    }

    // ���콺�� UI ���� �ִ��� Ȯ���ϴ� �޼���
    private bool IsMouseOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    void MouseToRotatePosition()
    {
        // ���콺 �̵� �� ��ǥ ��������
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        // ���콺 ��ġ�κ��� ������ ����ĳ��Ʈ�� ���� ���� ����
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDistance);

        // ���콺 Ŭ�� ���� �� Scope ������Ʈ ��ġ ������Ʈ
        if (Input.GetMouseButton(0) && !IsMouseOverUI()) // ���� ���콺 ��ư Ŭ�� ���� ��
        {
            if (isReloading) // ���ε� ���� ��� �ڷ�ƾ�� �ߴ��ϰ� ���� ������Ʈ
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
                    Scope.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y, hit.transform.position.z - 0.5f); // �������� �� ��ġ�� �̵�

                    if (fireCooldown <= 0)
                    {
                        Attack(hit.transform.GetComponent<Enemy>());
                        fireCooldown = player.currentRateOfFire; // �߻� �ӵ� �ʱ�ȭ
                    }

                    return; // ���� ����� ���� �����ϰ� �������� ����
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

            // �ڷ�ƾ�� �����Ͽ� ���ε带 ó��
            StartCoroutine(RealoadTimeCheck(player.currentReloadTime));
        }
    }

    public void Attack(Enemy targetEnemy)
    {
        if (player.currentAmmo <= 0)
        {
            //���� üũ
            player.currentAmmo = 0;
            Debug.Log("źâ 0��");
            return;
        }

        Debug.Log("���� ��");

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
        isReloading = true; // ���ε� ���� ���� ����

        Image reloadFillImage = RealoadBar.transform.GetChild(0).GetComponent<Image>();
        reloadFillImage.fillAmount = 0; // ���ε� ���� �� �ʱ�ȭ

        while (ReloadTime > 0)
        {
            ReloadTime -= Time.deltaTime;
            reloadFillImage.fillAmount = 1 - (ReloadTime / player.currentReloadTime); // ���ε� ����� ������Ʈ
            yield return null; // ���� �����ӱ��� ���
        }

        // ���ε尡 �Ϸ�Ǿ��� ���� ���� ���ε� ó��
        if (isReloading)
        {
            reloadFillImage.fillAmount = 1; // ���ε� �Ϸ�
            Reload(); // ���ε� �Ϸ� �� źâ ����
            RealoadBar.gameObject.SetActive(false);
        }

        isReloading = false; // ���ε� ���� ����
    }

    //SG�� ������ ����ѱ������ ����� ���� �Լ�
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

    void AmmoUIUpdate()
    {
        int Index = gm.PlayerIndex;
        float fill = player.currentAmmo / gm.MaxAmmos[Index];

        AmmoText.text = player.currentAmmo.ToString();
        AmmoBar.fillAmount = fill;
    }
}
