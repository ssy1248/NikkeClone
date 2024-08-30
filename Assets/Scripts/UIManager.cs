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

    //AutoAttack 활성화 하면서 Player 스크립트에서 AutoAttack함수로 변화
    public void AutoAttackButtonPress(Button btn)
    {
        // 버튼 색상 변경
        Color color = btn.image.color;
        // 자식 이미지 컴포넌트 찾기
        Image child = btn.transform.GetChild(0).GetComponent<Image>();
        if (isAutoAttackButtonActive)
        {
            // 알파 값을 0으로 설정 (활성화 상태)
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
            // 알파 값을 1로 설정 (비활성화 상태)
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

        // 버튼 상태 반전
        isAutoAttackButtonActive = !isAutoAttackButtonActive;
    }

    //자동 버스트
    public void AutoBurstButtonPress(Button btn)
    {
        // 버튼 색상 변경
        Color color = btn.image.color;
        // 자식의 Text에 접근
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

    //게임 멈추기 -> PauseScreen 활성화 하기
    public void PauseGame()
    {
        
    }

    //Resume 버튼 누르면 -> PauseScreen 비활성화
    public void ResumeGame()
    {

    }
}
