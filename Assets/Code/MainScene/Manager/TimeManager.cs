using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public Slider OxygeSlider;
    public UnityEvent<int> OnTierChange; // Ƽ� ����� �� �߻��ϴ� �̺�Ʈ

    [Header("Times")]
    public float NextExpTierTime = 180f; // ���� ����ġ Ƽ������� �ð�
    public float PlayerDeathTime = 900f;

    public int CurrentTier = 0; // ���� ����ġ ������ Ƽ��
    public int DebuffIndex = 0;

    MonsterSpawner monsterSpawner;

    float monsterSpawnTime = 5f; // ���� ��Ÿ��
    float regenerationTime = 3f; // ����ġ ���� ���� ��Ÿ��
    float gameTime = 0f;
    float oxygeTime = 0f;
    float timeSinceLastSpawn = 0f;

    void Awake()
    {
        Instance = this;
        OnTierChange = new UnityEvent<int>();
        monsterSpawner = MonsterSpawner.Instance;
    }

    void Start()
    {
        OxygeSlider.maxValue = PlayerDeathTime;
        OxygeSlider.value = PlayerDeathTime;
        StartCoroutine(ExpOrbSpawner.Instance.GenerateRandomSpawnLocations(regenerationTime));
        InvokeRepeating("UpdateOxygeSlider", 1f, 1f);
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        gameTime += Time.deltaTime;
        oxygeTime += Time.deltaTime;

        if (timeSinceLastSpawn >= monsterSpawnTime && monsterSpawner.MonstersSpawned < monsterSpawner.MaxMonsters)
        {
            monsterSpawner.SpawnMonster();
            timeSinceLastSpawn = 0f;
        }

        if (gameTime >= NextExpTierTime) // Ƽ�� ���� ����
        {
            CurrentTier = Mathf.Min(CurrentTier + 1, 5); // �ִ� 5Ƽ�����
            gameTime = 0f;
            OnTierChange.Invoke(CurrentTier); // Ƽ�� ���� �̺�Ʈ ȣ��
        }

        if (oxygeTime >= PlayerDeathTime)
        {
            GameManager.Instance.GameOver();
        }
    }

    // ���� Ƽ����� ���� �ð��� ��ȯ�ϴ� �Լ�
    public float GetTimeUntilNextTier()
    {
        return NextExpTierTime - gameTime;
    }

    void UpdateOxygeSlider()
    {
        OxygeSlider.value = PlayerDeathTime - oxygeTime;
    }
}
