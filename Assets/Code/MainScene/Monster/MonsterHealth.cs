using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    public float maxHP;
    public float currentHP;

    bool isDead = false;

    MonsterSpawner spawner;

    void Start()
    {
        spawner = MonsterSpawner.Instance;
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHP = Mathf.Max(0, currentHP - damage);

        if (currentHP <= 0 && !isDead)
        {
            Die();
        }
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
