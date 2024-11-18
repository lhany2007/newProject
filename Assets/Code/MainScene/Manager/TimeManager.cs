using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public UnityEvent<int> OnTierChange; // Ƽ� ����� �� �߻��ϴ� �̺�Ʈ
    public MonsterSpawner monsterSpawner;
    public PlayerHealth playerHealth;

    [Header("Times")]
    public float NextExpTierTime = 180f; // ���� ����ġ Ƽ������� �ð�
    public float PlayerDeathTime = 900f;

    public int CurrentTier = 0; // ���� ����ġ ������ Ƽ��
    public int DebuffIndex = 0;

    Collision2D collision2D = null;

    float monsterSpawnTime = 5f; // ���� ��Ÿ��
    float regenerationTime = 3f; // ����ġ ���� ���� ��Ÿ��
    float gameTime = 0f;
    float timeSinceLastSpawn = 0f;

    void Awake()
    {
        Instance = this;
        OnTierChange = new UnityEvent<int>();
    }

    void Start()
    {
        StartCoroutine(ExpOrbSpawner.Instance.GenerateRandomSpawnLocations(regenerationTime));
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        gameTime += Time.deltaTime;

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

        playerHealth.TakeDamage(0.001f, collision2D); // ��Ұ� �پ��
    }

    // ���� Ƽ����� ���� �ð��� ��ȯ�ϴ� �Լ�
    public float GetTimeUntilNextTier()
    {
        return NextExpTierTime - gameTime;
    }
}
