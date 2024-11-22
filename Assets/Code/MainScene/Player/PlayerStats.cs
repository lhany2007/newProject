using UnityEngine;
using System.Collections;
using static UIManager;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    private UIManager uiManager;
    private SliderManager sliderManager;
    private TextManager textManager;

    // HP
    public float invincibilityDuration = 0.01f;
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
            Debug.LogError("playerStats Instance�� �̹� �Ҵ��");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeReferences();
        InitializeStats();
    }

    private void InitializeReferences()
    {
        uiManager = UIManager.Instance;
        sliderManager = uiManager.sliderManager;
        textManager = uiManager.textManager;
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
            int damage = MonsterSpawner.Instance.MonsterDamageDictionary[collision.gameObject.name];
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
        sliderManager.SliderDictionary["HP"].value = currentHP;

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
        sliderManager.SliderDictionary["XP"].value = xpRatio;
    }

    private void UpdateLevelText()
    {
        textManager.TextUpdate("Level", CurrentLevel.ToString());
    }
}