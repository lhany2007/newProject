using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public UnityEvent<int> OnTierChange; // Ƽ� ����� �� �߻��ϴ� �̺�Ʈ

    [Header("Times")]
    public float NextExpTierTime = 180f; // ���� ����ġ Ƽ������� �ð�
    public float PlayerDeathTime = 900f;

    public int CurrentTier = 0; // ���� ����ġ ������ Ƽ��
    public int DebuffIndex = 0;
    public int MonsterTier = 0;

    float monsterSpawnTime = 5f; // ���� ��Ÿ��
    float regenerationTime = 3f; // ����ġ ���� ���� ��Ÿ��
    float gameTime = 0f;
    float timeSinceLastSpawn = 0f;
    float NextMonsterTime = 0f;
    
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
        NextMonsterTime += Time.deltaTime;

        if (timeSinceLastSpawn >= monsterSpawnTime &&
            MonsterSpawner.Instance.MonstersSpawned < MonsterSpawner.Instance.MaxMonsters)
        {
            MonsterSpawner.Instance.SpawnMonster();
            timeSinceLastSpawn = 0f;
        }

        if (NextMonsterTime >= 120f)
        {
            NextMonsterTime = 0f;
            MonsterTier++;
        }

        if (gameTime >= NextExpTierTime) // Ƽ�� ���� ����
        {
            CurrentTier = Mathf.Min(CurrentTier + 1, 5); // �ִ� 5Ƽ�����
            gameTime = 0f;
            OnTierChange.Invoke(CurrentTier); // Ƽ�� ���� �̺�Ʈ ȣ��
        }

        PlayerStats.Instance.TakeDamage(0.001f, transform.position, false); // ��Ұ� �پ��
    }

    // ���� Ƽ����� ���� �ð��� ��ȯ�ϴ� �Լ�
    public float GetTimeUntilNextTier()
    {
        return NextExpTierTime - gameTime;
    }
}
