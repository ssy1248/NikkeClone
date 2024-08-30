using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameManager gm;

    public AmmoUiManager ammoUIManager;
    public BurstManager burstManager;
    public BurstCooldownManager burstCooldownManager;

    private bool isAutoAttackButtonActive = true;
    private bool isAutoBurstButtonActive = true;

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();

        burstManager.burstCooldownManager = burstCooldownManager;
        burstManager.Awake();
    }

    void Start()
    {
        GunImageSetting();
    }

    void Update()
    {
        ammoUIManager.Update();
        burstManager.Update();
    }

    void GunImageSetting()
    {
        for (int i = 0; i < gm.PlayerObjects.Count; i++)
        {
            ammoUIManager.InputGunImage[i].sprite = gm.PlayerObjects[i].GetComponent<PlayerStat>().currentGunImage.sprite;
        }
    }

    //AutoAttack Ȱ��ȭ �ϸ鼭 Player ��ũ��Ʈ���� AutoAttack�Լ��� ��ȭ
    public void AutoAttackButtonPress(Button btn)
    {
        // ��ư ���� ����
        Color color = btn.image.color;
        // �ڽ� �̹��� ������Ʈ ã��
        Image child = btn.transform.GetChild(0).GetComponent<Image>();
        if (isAutoAttackButtonActive)
        {
            // ���� ���� 0���� ���� (Ȱ��ȭ ����)
            color.a = 0;
            btn.image.color = color;

            if (child != null)
            {
                Color childColor = child.color;
                childColor = new Color(255, 0, 0);
                child.color = childColor; 
            }

            gm.isAutoAttack = true;
        }
        else
        {
            // ���� ���� 1�� ���� (��Ȱ��ȭ ����)
            color.a = 1;
            btn.image.color = color;

            if (child != null)
            {
                Color childColor = child.color;
                childColor = new Color(0, 0, 0);
                child.color = childColor;
            }

            gm.isAutoAttack = false;
        }

        // ��ư ���� ����
        isAutoAttackButtonActive = !isAutoAttackButtonActive;
    }

    //�ڵ� ����Ʈ
    public void AutoBurstButtonPress(Button btn)
    {
        // ��ư ���� ����
        Color color = btn.image.color;
        // �ڽ��� Text�� ����
        TextMeshProUGUI childtext = btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if (isAutoBurstButtonActive)
        {
            color.a = 0;
            btn.image.color = color;

            if(childtext != null)
            {
                Color childColor = childtext.color;
                childColor = new Color(255, 0, 0);
                childtext.color = childColor;
            }

            gm.isAutoBurst = true;
        }
        else
        {
            color.a = 1;
            btn.image.color = color;

            if (childtext != null)
            {
                Color childColor = childtext.color;
                childColor = new Color(0, 0, 0);
                childtext.color = childColor;
            }

            gm.isAutoBurst = false;
        }

        isAutoBurstButtonActive = !isAutoBurstButtonActive;
    }

    //���� ���߱� -> PauseScreen Ȱ��ȭ �ϱ�
    public void PauseGame()
    {
        
    }

    //Resume ��ư ������ -> PauseScreen ��Ȱ��ȭ
    public void ResumeGame()
    {

    }
}
