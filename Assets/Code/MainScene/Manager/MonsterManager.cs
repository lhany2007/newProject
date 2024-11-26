using System;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private static MonsterManager _instance;
    public static MonsterManager Instance
    {
        get => _instance;
        private set => _instance = value;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AttackPlayerOnCollision(float damage, Vector3 myPos, bool isKnockback)
    {
        PlayerStats.Instance.TakeDamage(damage, myPos, isKnockback);
    }
}
