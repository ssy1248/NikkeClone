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
}
