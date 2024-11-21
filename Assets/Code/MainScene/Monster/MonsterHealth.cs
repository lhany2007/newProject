using System.Collections;
using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    public float maxHP;
    public float currentHP;
    public float invincibilityDuration = 0.01f; // 무적 시간
    public float knockbackForce = 5f; // 넉백 힘

    bool isDead = false;
    bool isInvincible = false; // 무적 상태 플래그

    MonsterSpawner spawner;
    Rigidbody2D rb;
    HitFlash hitFlash;

    void Start()
    {
        spawner = MonsterSpawner.Instance;
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
        hitFlash = GetComponent<HitFlash>();
    }

    public void TakeDamage(float damage)
    {
        // 사망 상태이거나 무적 상태일 때 데미지 무효화
        if (isDead || isInvincible)
        {
            return;
        }

        currentHP = Mathf.Max(0, currentHP - damage);

        if (currentHP <= 0 && !isDead)
        {
            Die();
        }
        else
        {
            Vector3 knockbackDirection = (transform.position - PlayerMovement.Instance.transform.position).normalized;
            Knockback(knockbackDirection); // 넉백 적용

            StartCoroutine(InvincibilityDelay());

            hitFlash.TriggerFlash(); // 히트 플래시 호출
        }
    }

    public void Knockback(Vector3 direction)
    {
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
    }

    IEnumerator InvincibilityDelay()
    {
        isInvincible = true; // 무적 상태 시작
        yield return new WaitForSeconds(invincibilityDuration); // 지정된 시간 동안 대기
        isInvincible = false; // 무적 상태 종료
    }

    void Die()
    {
        isDead = true;

        if (spawner != null)
        {
            spawner.OnMonsterDestroyed();
        }

        // 경험치 스폰 로직 추가
        int currentTier = TimeManager.Instance.CurrentTier;
        int maxTier = ExpOrbSpawner.Instance.ExpOrbPrefabList.Count - 1;
        int spawnTier = Mathf.Clamp(currentTier + 1, 0, maxTier); // 현재 티어 +1 (최대 티어 제한)

        ExpOrbSpawner.Instance.SpawnExpOrb(transform.position, spawnTier); // 현재 위치에 경험치 스폰

        Destroy(gameObject); // 오브젝트 제거
    }
}
