using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    public EnemyScriptableObject enemy;

    public AttributeCodes currentCodes;
    public float currentMoveSpeed;
    public float currentHealth;
    public float currentDamage;
    public float currentDefece;

    public float MaxHealth;

    private void Awake()
    {
        currentCodes = enemy.Codes;
        currentMoveSpeed = enemy.MoveSpeed;
        currentHealth = enemy.MaxHealth;
        currentDamage = enemy.Damage;
        currentDefece = enemy.Defence;
        MaxHealth = currentHealth;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
