using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public MonsterSpawner monsterSpawner;

    private PlayerStats playerStats;

    [Header("Times")]
    public float NextExpTierTime = 180f; // 다음 경험치 티어까지의 시간
    public float PlayerDeathTime = 900f;

    public int DebuffIndex = 0;

    Collision2D collision2D = null;

    float monsterSpawnTime = 5f; // 스폰 쿨타임
    float regenerationTime = 3f; // 경험치 구슬 생성 쿨타임
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

        if (gameTime >= NextExpTierTime) // 티어 변경 조건
        {
            ExpOrbSpawner.Instance.CurrentTier = Mathf.Min(ExpOrbSpawner.Instance.CurrentTier + 1, 5); // 최대 5티어까지
            gameTime = 0f;
            ExpOrbSpawner.Instance.OnTierChange(ExpOrbSpawner.Instance.CurrentTier);
        }

        playerStats.TakeDamage(0.001f, collision2D); // 산소가 줄어듦
    }

    // 다음 티어까지 남은 시간을 반환하는 함수
    public float GetTimeUntilNextTier()
    {
        return NextExpTierTime - gameTime;
    }
}
