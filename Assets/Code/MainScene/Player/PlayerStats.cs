using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    private SliderManager sliderManager;
    private TextManager textManager;

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
        InitializeReferences();
        InitializeStats();
    }

    private void InitializeReferences()
    {
        sliderManager = UIManager.Instance.sliderManager;
        textManager = UIManager.Instance.textManager;
    }

    private void InitializeStats()
    {
        currentHP = MaxHP;
        CurrentXP = START_XP_VALUE;
        sliderManager.UpdateXPSlider(CurrentXP);
        UpdateLevelText();
    }

    // HP
    public void TakeDamage(float damage, Vector3 targetVec, bool shouldKnockback)
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
        // collision == null이면 TimeManager로부터 받는 데미지(넉백 X)
        if (shouldKnockback)
        {
            PlayerMovement.Instance.ApplyKnockback(transform.position, targetVec);
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
        if (collider.gameObject.CompareTag(Tags.XP))
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
        sliderManager.UpdateXPSlider(CurrentXP);
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

    private void UpdateLevelText()
    {
        textManager.TextUpdate("Level", CurrentLevel.ToString());
    }
}