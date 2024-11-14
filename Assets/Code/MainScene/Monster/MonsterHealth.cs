using System.Collections;
using System.Threading;
using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    public GameObject Player;

    public float maxHP;
    public float currentHP;
    public float invincibilityDuration = 0.01f; // ���� �ð�
    public float knockbackForce = 5f; // �˹� ��

    bool isDead = false;
    bool isInvincible = false; // ���� ���� �÷���

    MonsterSpawner spawner;

    void Start()
    {
        spawner = MonsterSpawner.Instance;
        currentHP = maxHP;
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
            Vector3 knockbackDirection = (transform.position - Player.transform.position).normalized;
            Knockback(knockbackDirection); // �˹� ����

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
