using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUiManager : MonoBehaviour
{
    GameManager gm;

    //�´� �ѱ⸦ ������� �̹���
    public List<Image> InputGunImage = new List<Image>();

    // ���� źâ ǥ���� �ؽ�Ʈ
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
            // ���� ź��� �ִ� ź���� ������� fillAmount ���
            float fillAmount = (float)player.currentAmmo / gm.MaxAmmos[playerIndex];

            // UI �̹����� fillAmount ������Ʈ
            InputGunImage[playerIndex].fillAmount = fillAmount;

            // ���� ź�� �ؽ�Ʈ ������Ʈ
            AmmoTexts[playerIndex].text = player.currentAmmo + " / " + gm.MaxAmmos[playerIndex];
        }
    }

    public void UpdateAutoAttackAmmoUI(PlayerStat player)
    {
        int playerIndex = player.PlayerIndex;
        if (playerIndex >= 0 && playerIndex < AmmoTexts.Count)
        {
            // ���� ź��� �ִ� ź���� ������� fillAmount ���
            float fillAmount = (float)player.currentAmmo / gm.MaxAmmos[playerIndex];

            // UI �̹����� fillAmount ������Ʈ
            InputGunImage[playerIndex].fillAmount = fillAmount;

            // ���� ź�� �ؽ�Ʈ ������Ʈ
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
