using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public MonsterSpawner monsterSpawner;

    private PlayerStats playerStats;

    [Header("Times")]
    public float NextExpTierTime = 180f; // ���� ����ġ Ƽ������� �ð�
    public float PlayerDeathTime = 900f;

    public int DebuffIndex = 0;

    Collision2D collision2D = null;

    float monsterSpawnTime = 5f; // ���� ��Ÿ��
    float regenerationTime = 3f; // ����ġ ���� ���� ��Ÿ��
    float gameTime = 0f;
    float timeSinceLastSpawn = 0f;
    float NextMonsterTime = 0f;
    
    void Start()
    {
        StartCoroutine(ExpOrbSpawner.Instance.GenerateRandomSpawnLocations(regenerationTime));
        playerStats = PlayerStats.Instance;
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        gameTime += Time.deltaTime;
        NextMonsterTime += Time.deltaTime;

        if (timeSinceLastSpawn >= monsterSpawnTime &&
            monsterSpawner.MonstersSpawned < monsterSpawner.MaxMonsters)
        {
            monsterSpawner.SpawnMonster();
            timeSinceLastSpawn = 0f;
        }

        if (NextMonsterTime >= 120f)
        {
            NextMonsterTime = 0f;
            MonsterSpawner.Instance.MonsterTier++;
        }

        if (gameTime >= NextExpTierTime) // Ƽ�� ���� ����
        {
            ExpOrbSpawner.Instance.CurrentTier = Mathf.Min(ExpOrbSpawner.Instance.CurrentTier + 1, 5); // �ִ� 5Ƽ�����
            gameTime = 0f;
            ExpOrbSpawner.Instance.OnTierChange(ExpOrbSpawner.Instance.CurrentTier);
        }

        playerStats.TakeDamage(0.001f, collision2D); // ��Ұ� �پ��
    }

    // ���� Ƽ����� ���� �ð��� ��ȯ�ϴ� �Լ�
    public float GetTimeUntilNextTier()
    {
        return NextExpTierTime - gameTime;
    }
}
