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
            Debug.LogError("Scope ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�.");
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

            // �߻� �ӵ� �缳��
            if (fireCooldown >= 0)
            {
                fireCooldown -= Time.deltaTime;
            }
        }
    }

    void MouseToRotatePosition()
    {
        // ���콺 �̵� �� ��ǥ ��������
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        // ���콺 Ŭ�� ���� �� Scope ������Ʈ ��ġ ������Ʈ
        if (Input.GetMouseButton(0)) // ���� ���콺 ��ư Ŭ�� ���� ��
        {
            AmmoUIUpdate();
            Scope.SetActive(true);
            Scope.transform.position = mousePosition;

            // Raycast�� �� ������Ʈ ����
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    Scope.transform.position = hit.point; // Scope�� �� ��ġ�� �̵�
                    if (fireCooldown <= 0)
                    {
                        Attack(hit.transform.GetComponent<Enemy>());
                        fireCooldown = player.currentRateOfFire; // �߻� �ӵ� �ʱ�ȭ
                    }
                }
                else if(hit.transform.CompareTag("Breakable"))
                {
                    //�ǹ�
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
            //���� üũ
            player.currentAmmo = 0;
            Debug.Log("źâ 0��");
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
