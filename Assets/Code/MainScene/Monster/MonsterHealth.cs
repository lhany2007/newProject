using System;
using System.Collections;
using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    public float maxHP;
    public float currentHP;

    [Header("Combat Settings")]
    [SerializeField] private float invincibilityDuration = 0.01f; // 무적시간
    [SerializeField] private float knockbackForce = 5f; // 넉백

    private bool isDead;
    private bool isInvincible;

    private MonsterSpawner spawner;
    private Rigidbody2D rb;
    private HitFlash hitFlash;
    private Animator animator;

    private void Awake()
    {
        GetComponents();
    }

    private void GetComponents()
    {
        spawner = MonsterSpawner.Instance;
        rb = GetComponent<Rigidbody2D>();
        hitFlash = GetComponent<HitFlash>();
        animator = GetComponent<Animator>();
    }

    public void Initialize(float maxHealth)
    {
        maxHP = maxHealth;
        currentHP = maxHealth;
        isDead = false;
        isInvincible = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInvincible) return;

        ApplyDamage(damage);
        HandleDamageEffects();
    }

    private void ApplyDamage(float damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);

        if (currentHP <= 0 && !isDead)
        {
            Die();
            return;
        }
    }

    private void HandleDamageEffects()
    {
        if (isDead)
        {
            return;
        }

        ApplyKnockback();
        StartCoroutine(InvincibilityDelay());
        hitFlash?.TriggerFlash();
    }

    private void ApplyKnockback()
    {
        if (rb == null)
        {
            throw new NullReferenceException("Rigidbody2D is null");
        }
        AnimationManager.Instance.StopAnimation(animator);
        KnockbackManager.Instance.ApplyKnockback(rb, knockbackForce, transform.position, PlayerMovement.Instance.transform.position);
        AnimationManager.Instance.StartAnimation(animator);
    }

    /// <summary>
    /// 무적시간
    /// </summary>
    private IEnumerator InvincibilityDelay()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private void Die()
    {
        isDead = true;
        spawner?.OnMonsterDestroyed();
        SpawnExpOrb();
        Destroy(gameObject);
    }


    /// <summary>
    /// 경험치 드롭
    /// </summary>
    private void SpawnExpOrb()
    {
        int currentTier = TimeManager.Instance.CurrentTier;
        int maxTier = ExpOrbSpawner.Instance.ExpOrbPrefabList.Count - 1;
        int spawnTier = Mathf.Clamp(currentTier + 1, 0, maxTier);

        ExpOrbSpawner.Instance.SpawnExpOrb(transform.position, spawnTier);
    }
}