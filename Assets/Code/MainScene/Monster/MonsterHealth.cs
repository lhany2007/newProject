using System.Collections;
using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    public float maxHP;
    public float currentHP;
    public float invincibilityDuration = 0.01f; // ���� �ð�
    public float knockbackForce = 5f; // �˹� ��

    bool isDead = false;
    bool isInvincible = false; // ���� ���� �÷���

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
        // ��� �����̰ų� ���� ������ �� ������ ��ȿȭ
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
            Knockback(knockbackDirection); // �˹� ����

            StartCoroutine(InvincibilityDelay());

            hitFlash.TriggerFlash(); // ��Ʈ �÷��� ȣ��
        }
    }

    void Knockback(Vector3 direction)
    {
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
    }

    IEnumerator InvincibilityDelay()
    {
        isInvincible = true; // ���� ���� ����
        yield return new WaitForSeconds(invincibilityDuration); // ������ �ð� ���� ���
        isInvincible = false; // ���� ���� ����
    }

    void Die()
    {
        isDead = true;

        if (spawner != null)
        {
            spawner.OnMonsterDestroyed();
        }

        Destroy(gameObject); // ������Ʈ ����
    }
}
