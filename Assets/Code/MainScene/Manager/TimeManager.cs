using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public UnityEvent<int> OnTierChange; // 티어가 변경될 때 발생하는 이벤트
    public MonsterSpawner monsterSpawner;
    public PlayerHealth playerHealth;

    [Header("Times")]
    public float NextExpTierTime = 180f; // 다음 경험치 티어까지의 시간
    public float PlayerDeathTime = 900f;

    public int CurrentTier = 0; // 현재 경험치 구슬의 티어
    public int DebuffIndex = 0;

    Collision2D collision2D = null;

    float monsterSpawnTime = 5f; // 스폰 쿨타임
    float regenerationTime = 3f; // 경험치 구슬 생성 쿨타임
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

        if (gameTime >= NextExpTierTime) // 티어 변경 조건
        {
            CurrentTier = Mathf.Min(CurrentTier + 1, 5); // 최대 5티어까지
            gameTime = 0f;
            OnTierChange.Invoke(CurrentTier); // 티어 변경 이벤트 호출
        }

        playerHealth.TakeDamage(0.001f, collision2D); // 산소가 줄어듦
    }

    // 다음 티어까지 남은 시간을 반환하는 함수
    public float GetTimeUntilNextTier()
    {
        return NextExpTierTime - gameTime;
    }
}
