using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    public PlayerScriptableObject player;
    public int PlayerIndex;
    
    public Weapons currentWeapon;
    public Image currentGunImage;
    public Types currentTypes;
    public AttributeCodes currentCodes;
    public Texture2D currentCodeImage;
    public BurstType currentBurst;
    public float currentBurstCoolTime;
    public float currentAmmo;
    public float currentReloadTime;
    public float currentRateOfFire;
    public float currentHealth;
    public float currentDefence;
    public float currentDamage;

    private void Awake()
    {
        currentWeapon = player.Weapons;
        currentGunImage = player.GunImage;
        currentTypes = player.Types;
        currentCodes = player.Code;
        currentCodeImage = player.CodeImage;
        currentBurst = player.Burst;
        currentBurstCoolTime = player.BurstCoolTime;
        currentAmmo = player.MaxAmmo;
        currentReloadTime = player.ReloadTime;
        currentRateOfFire = player.RateOfFire;
        currentHealth = player.MaxHealth;
        currentDefence = player.Defence;
        currentDamage = player.Damage;
    }
}
