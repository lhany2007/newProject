using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    // HP
    private float invincibilityDuration = 0.01f; // 무적시간
    public int MaxHP = 1000;
    public float currentHP;
    public const float START_HP_Value = 1000f;
    private bool isInvincible = false;
    private bool isDead = false;

    // XP
    public int CurrentLevel = 1;
    public float CurrentXP = 0f;
    public float XpSliderMaxValue = 30f;
    private const float EXP_MAX_VALUE_INCREASE = 0.5f;
    public const float START_XP_VALUE = 0f;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("playerStats Instance가 이미 할당됨");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeStats();
    }

    private void InitializeStats()
    {
        currentHP = MaxHP;

        CurrentXP = START_XP_VALUE;
        UpdateXPUI();
        UpdateLevelText();
    }

    // HP
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(Tags.MONSTER_LAYER))
        {
            float damage = MonsterManager.Instance.monsterStats.stats.Damage(MonsterManager.Instance.GetMonsterNameIndex(collision.gameObject.tag));
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
        UIManager.Instance.sliderManager.SliderDictionary["HP"].value = currentHP;

        if (currentHP <= 0 && !isDead)
        {
            // Die();
        }
        // collision == null이면 TimeManager로부터 받는 데미지(넉백 X)
        if (collision != null)
        {
            PlayerMovement.Instance.ApplyKnockback(transform.position, collision.transform.position);
            StartCoroutine(InvincibilityDelay());
        }
    }

    private IEnumerator InvincibilityDelay()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    // XP
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(Tags.EXP_TAG))
        {
            int orbType = int.Parse(collider.gameObject.name.Split('_')[1]);
            CollectExpOrb(collider.gameObject, orbType);
        }
    }

    public void CollectExpOrb(GameObject orb, int orbType)
    {
        float xpIncrease = ExpOrbSpawner.Instance.XPIncrease[orbType];
        CurrentXP += xpIncrease;

        CheckAndProcessLevelUp();
        UpdateXPUI();

        ExpOrbSpawner.Instance.CollectExpOrb(orb, orbType);
    }

    private void CheckAndProcessLevelUp()
    {
        while (CurrentXP >= XpSliderMaxValue)
        {
            CurrentXP -= XpSliderMaxValue;
            XpSliderMaxValue += EXP_MAX_VALUE_INCREASE;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        CurrentLevel++;
        UpdateLevelText();
    }

    private void UpdateXPUI()
    {
        float xpRatio = CurrentXP / XpSliderMaxValue;
        UIManager.Instance.sliderManager.SliderDictionary["XP"].value = xpRatio;
    }

    private void UpdateLevelText()
    {
        UIManager.Instance.textManager.TextUpdate("Level", CurrentLevel.ToString());
    }
}