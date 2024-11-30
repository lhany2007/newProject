using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static MonsterSpawner _instance;
    public static MonsterSpawner Instance
    {
        get => _instance;
        private set => _instance = value;
    }

    [Header("Spawn Settings")]
    [SerializeField] private Transform player; // 플레이어 Transform 참조
    [SerializeField] private float spawnRadius = 15f; // 플레이어 주변 스폰 반경
    public int MaxMonsters = 300; // 최대 스폰 가능한 몬스터 수

    public int MonstersSpawned = 0; // 현재 스폰된 몬스터 수

    private readonly System.Random random = new();
    private Transform spawnParent; // 스폰된 몬스터의 부모 Transform

    public MonsterData monsterData; // 몬스터 데이터 ScriptableObject 참조

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        spawnParent = transform; // 스폰된 몬스터들의 부모를 설정
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag(Tags.Player).transform; // 플레이어를 태그로 찾기
        }
        if (player == null)
        {
            throw new System.NullReferenceException("Player is null");
        }
    }

    // 플레이어 주변의 랜덤한 위치에 몬스터를 스폰
    public GameObject SpawnMonster()
    {
        // 최대 몬스터 수를 초과하면 스폰 불가
        if (MonstersSpawned >= MaxMonsters)
        {
            return null;
        }

        int monsterTier = CalculateMonsterTier(); // 몬스터 티어 계산
        string monsterName = monsterData.GetMonsterName(monsterTier); // 티어에 해당하는 몬스터 이름 가져오기
        Vector3 position = GetRandomSpawnPosition(); // 스폰 위치 계산

        GameObject monsterPrefab = monsterData.GetMonsterPrefab(monsterName);

        // 몬스터 인스턴스화 및 초기화
        GameObject monster = Instantiate(monsterPrefab, position, Quaternion.identity, spawnParent);
        InitializeMonster(monster, monsterTier);
        return monster;
    }

    // 플레이어 주변 랜덤한 위치 계산
    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomOffset = Random.insideUnitCircle * spawnRadius; // 랜덤한 원형 위치 계산
        return player.position + new Vector3(randomOffset.x, randomOffset.y, 0f); // 플레이어 위치 기준으로 오프셋 적용
    }

    // 게임 시간 및 확률에 따라 몬스터 티어 계산
    private int CalculateMonsterTier()
    {
        int monsterTier = TimeManager.Instance.MonsterTier; // 현재 시간에 따른 기본 티어

        int chance = random.Next(0, 10); // 0~9 범위의 난수 생성
        monsterTier += (chance < 7) ? 0 : 1; // 30% 확률로 티어 증가

        return monsterTier;
    }

    // 스폰된 몬스터를 초기화
    private void InitializeMonster(GameObject monster, int tier)
    {
        var monsterHealth = monster.GetComponent<MonsterHealth>();
        if (monsterHealth == null)
        {
            throw new System.NullReferenceException("monsterHealth is null");
        }
        monster.name = monsterData.GetMonsterName(tier);// 몬스터 이름 설정
        var stats = monsterData.GetMonsterStats(monster.name);
        monsterHealth.Initialize(stats.MaxHP); // 체력 초기화
        MonstersSpawned++;
    }

    // 몬스터가 파괴될 때 호출되어 카운터 갱신
    public void OnMonsterDestroyed()
    {
        MonstersSpawned = Mathf.Max(0, MonstersSpawned - 1); // 최소값 0으로 제한
    }
}
