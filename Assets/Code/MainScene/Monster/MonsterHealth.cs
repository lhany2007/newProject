using System.Collections;
using System.Threading;
using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    public GameObject Player;

    public float maxHP;
    public float currentHP;
    public float invincibilityDuration = 0.01f; // 무적 시간
    public float knockbackForce = 5f; // 넉백 힘

    bool isDead = false;
    bool isInvincible = false; // 무적 상태 플래그

    MonsterSpawner spawner;

    void Start()
    {
        spawner = MonsterSpawner.Instance;
        currentHP = maxHP;
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
            Vector3 knockbackDirection = (transform.position - Player.transform.position).normalized;
            Knockback(knockbackDirection); // 넉백 적용

            StartCoroutine(InvincibilityDelay());
        }
    }

    void Knockback(Vector3 direction)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction.normalized * knockbackForce, ForceMode.Impulse);
        }
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

        Destroy(gameObject); // 오브젝트 제거
    }
}
