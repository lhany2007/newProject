using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    public Slider HPSlider;

    public float invincibilityDuration = 0.01f; // 무적 시간

    public int MAX_HP = 1000;
    public float currentHP;

    const string MONSTER_LAYER = "Monster";

    bool isInvincible = false; // 무적 상태 플래그
    bool isDead = false;

    void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        HPSlider.maxValue = MAX_HP;
        HPSlider.value = MAX_HP;
        currentHP = MAX_HP;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(MONSTER_LAYER))
        {
            int damage = MonsterSpawner.Instance.MonsterDamageDictionary[collision.gameObject.name];
            Debug.Log($"{damage}의 피해를 입었대요~");
            TakeDamage(damage, collision);
        }
    }

    public void TakeDamage(float damage, Collision2D collision)
    {
        if (isDead || isInvincible)
        {
            return;
        }

        currentHP -= damage;
        HPSlider.value = currentHP;
        if (currentHP <= 0 && !isDead)
        {
            // Die();
        }
        else if (collision != null)
        {
            Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized;
            PlayerMovement.Instance.ApplyKnockback(knockbackDirection);

            StartCoroutine(InvincibilityDelay());
        }
    }

    IEnumerator InvincibilityDelay()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}
