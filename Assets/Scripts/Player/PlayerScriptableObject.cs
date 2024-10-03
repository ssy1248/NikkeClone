using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Weapons
{
    SG, //샷건
    MG, //머신건
    AR, //자동소총
    SR, //저격소총
    SMG, //기관단총
    RL, //런처
}

public enum Types
{
    Normal, //일반형
    Charge, //차지형
}

public enum AttributeCodes
{
    Fire,
    Wind,
    Metal,
    Electro,
    Water,
}

public enum BurstType
{
    Burst1, //1버트스
    Burst2, //2버스트
    Burst3, //3버스트
}

[CreateAssetMenu(fileName = "PlayerScriptableObject", menuName = "ScriptableObject/Player")]
public class PlayerScriptableObject : ScriptableObject
{
    [SerializeField]
    Weapons weapon;
    public Weapons Weapons { get => weapon; set => weapon = value; }

    [SerializeField]
    Image gunImage;
    public Image GunImage { get => gunImage; set => gunImage = value; }

    [SerializeField]
    Types types;
    public Types Types { get => types; set => types = value; }

    [SerializeField]
    AttributeCodes code;
    public AttributeCodes Code { get => code; set => code = value; }

    [SerializeField]
    Texture2D codeImage;
    public Texture2D CodeImage { get => codeImage; set => codeImage = value; }

    [SerializeField]
    BurstType burst;
    public BurstType Burst { get=> burst; set => burst = value; }

    [SerializeField]
    float burstCoolTime;
    public float BurstCoolTime { get => burstCoolTime; set => burstCoolTime = value; }

    [SerializeField]
    float maxAmmo;
    public float MaxAmmo { get => maxAmmo; set => maxAmmo = value; }

    [SerializeField]
    float reloadTime;
    public float ReloadTime { get => reloadTime; set => reloadTime = value; }

    [SerializeField]
    float rateOfFire;
    public float RateOfFire { get => rateOfFire; set => rateOfFire = value; }

    [SerializeField]
    float maxHealth;
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }

    [SerializeField]
    float defence;
    public float Defence { get => defence; set => defence = value; }

    [SerializeField]
    float damage;
    public float Damage { get => damage; set => damage = value; }
}
