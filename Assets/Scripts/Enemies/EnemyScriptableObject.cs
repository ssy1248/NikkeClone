using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObject/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    [SerializeField]
    AttributeCodes codes;
    public AttributeCodes Codes { get => codes; set => codes = value; }

    [SerializeField]
    float moveSpeed;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

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
