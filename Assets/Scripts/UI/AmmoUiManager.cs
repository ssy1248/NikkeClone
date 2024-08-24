using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUiManager : MonoBehaviour
{
    GameManager gm;

    //맞는 총기를 집어넣을 이미지
    public List<Image> InputGunImage = new List<Image>();

    // 남은 탄창 표시할 텍스트
    public List<Text> AmmoTexts = new List<Text>();

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void Update()
    {
        AmmoTextSet();
    }

    void AmmoTextSet()
    {
        for (int i = 0; i < gm.PlayerObjects.Count; i++)
        {
            AmmoTexts[i].text = gm.MaxAmmos[i] + " / " + gm.PlayerObjects[i].GetComponent<PlayerStat>().currentAmmo;
        }
    }

    public void UpdateAttckAmmoUI(PlayerStat player)
    {
        int playerIndex = gm.PlayerIndex;
        if (playerIndex >= 0 && playerIndex < AmmoTexts.Count)
        {
            // 현재 탄약과 최대 탄약을 기반으로 fillAmount 계산
            float fillAmount = (float)player.currentAmmo / gm.MaxAmmos[playerIndex];

            // UI 이미지의 fillAmount 업데이트
            InputGunImage[playerIndex].fillAmount = fillAmount;

            // 남은 탄약 텍스트 업데이트
            AmmoTexts[playerIndex].text = player.currentAmmo + " / " + gm.MaxAmmos[playerIndex];
        }
    }

    public void UpdateAutoAttackAmmoUI(PlayerStat player)
    {
        int playerIndex = player.PlayerIndex;
        if (playerIndex >= 0 && playerIndex < AmmoTexts.Count)
        {
            // 현재 탄약과 최대 탄약을 기반으로 fillAmount 계산
            float fillAmount = (float)player.currentAmmo / gm.MaxAmmos[playerIndex];

            // UI 이미지의 fillAmount 업데이트
            InputGunImage[playerIndex].fillAmount = fillAmount;

            // 남은 탄약 텍스트 업데이트
            AmmoTexts[playerIndex].text = player.currentAmmo + " / " + gm.MaxAmmos[playerIndex];
        }
    }

    public void UpdateReloadAmmoUI(PlayerStat player)
    {
        int playerIndex = gm.PlayerIndex;
        if (playerIndex >= 0 && playerIndex < AmmoTexts.Count)
        {
            AmmoTexts[playerIndex].text = gm.MaxAmmos[playerIndex] + " / " + player.currentAmmo;
        }
    }
}
