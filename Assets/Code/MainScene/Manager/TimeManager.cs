using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("Times")]
    public float GameTime = 0f;
    public float OxygeTime = 0f;
    public float NextTierTime = 180f; // ���� Ƽ������� �ð�
    public float PlayerDeathTime = 900f;

    public int CurrentTier = 0; // ���� ����ġ ������ Ƽ��
    public int DebuffIndex = 0;

    public Slider OxygeSlider;

    public UnityEvent<int> onTierChange; // Ƽ� ����� �� �߻��ϴ� �̺�Ʈ

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (onTierChange == null)
        {
            onTierChange = new UnityEvent<int>();
        }
    }

    void Start()
    {
        OxygeSlider.maxValue = PlayerDeathTime;
        OxygeSlider.value = PlayerDeathTime;
        InvokeRepeating("UpdateOxygeSlider", 1f, 1f);
    }

    void Update()
    {
        GameTime += Time.deltaTime;
        OxygeTime += Time.deltaTime;

        if (GameTime >= NextTierTime) // Ƽ�� ���� ����
        {
            CurrentTier = Mathf.Min(CurrentTier + 1, 5); // �ִ� 5Ƽ�����
            GameTime = 0f;
            onTierChange.Invoke(CurrentTier); // Ƽ�� ���� �̺�Ʈ ȣ��
        }

        if (OxygeTime >= PlayerDeathTime)
        {
            GameManager.Instance.GameOver();
        }
    }

    // ���� Ƽ����� ���� �ð��� ��ȯ�ϴ� �Լ�
    public float GetTimeUntilNextTier()
    {
        return NextTierTime - GameTime;
    }

    void UpdateOxygeSlider()
    {
        OxygeSlider.value = PlayerDeathTime - OxygeTime;
    }
}
