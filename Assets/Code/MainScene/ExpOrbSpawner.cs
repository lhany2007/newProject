using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExpOrbSpawner : MonoBehaviour
{
    public static ExpOrbSpawner Instance;

    [SerializeField] Transform player;
    [SerializeField] List<GameObject> expOrbPrefabs;

    int expSpawnCount = 3; // 한 번에 생성할 경험치 오브 수
    float regenerationTime = 2f; // 오브 생성 주기
    float maximumSpawnDistance = 5f; // 최대 생성 거리
    int initialPoolSize = 20; // 초기 풀 크기

    // 각 경험치 오브 타입에 대한 객체 풀
    Dictionary<int, Queue<GameObject>> expOrbPools;
    Transform poolContainer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        InitializeObjectPools();
    }

    void Start()
    {
        // 경험치 오브를 주기적으로 생성
        StartCoroutine(GenerateRandomSpawnLocations());
    }

    /// <summary>
    /// 구슬을 미리 인스턴스화
    /// </summary>
    void InitializeObjectPools()
    {
        // 각 타입의 경험치 오브를 위한 객체 풀 초기화
        expOrbPools = new Dictionary<int, Queue<GameObject>>();
        poolContainer = new GameObject("ExpOrbPool").transform;
        poolContainer.SetParent(transform);

        for (int i = 0; i < expOrbPrefabs.Count; i++)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int j = 0; j < initialPoolSize; j++)
            {
                CreateNewExpOrb(i, pool); // 새 오브 생성
            }
            expOrbPools[i] = pool; 
        }
    }

    void CreateNewExpOrb(int orbType, Queue<GameObject> pool)
    {
        if (orbType >= expOrbPrefabs.Count)
        {
            Debug.LogError($"잘못된 오브 타입: {orbType}. 최대 타입은 {expOrbPrefabs.Count - 1}");
            return;
        }

        GameObject orb = Instantiate(expOrbPrefabs[orbType], poolContainer); // 인스턴스화 
        orb.SetActive(false);
        pool.Enqueue(orb);
    }

    GameObject GetExpOrbFromPool(int orbType)
    {
        if (!expOrbPools.TryGetValue(orbType, out Queue<GameObject> pool))
        {
            Debug.LogError($"오브 타입 {orbType}에 대한 풀이 없음");
            return null;
        }

        if (pool.Count == 0)
        {
            CreateNewExpOrb(orbType, pool);
        }

        GameObject orb = pool.Dequeue();
        orb.SetActive(true);
        return orb;
    }

    /// <summary>
    /// 사용이 끝난 오브를 비활성화하고 풀에 반환
    /// </summary>
    void ReturnToPool(GameObject orb, int orbType)
    {
        orb.SetActive(false);
        expOrbPools[orbType].Enqueue(orb);
    }

    /// <summary>
    /// 주기적으로 랜덤 위치에 경험치 오브 생성
    /// </summary>
    IEnumerator GenerateRandomSpawnLocations()
    {
        WaitForSeconds wait = new WaitForSeconds(regenerationTime);
        while (true)    
        {
            SpawnExpOrbsOverTime();
            yield return wait;
        }
    }

    void SpawnExpOrbsOverTime()
    {
        int orbType = Mathf.Min(0, expOrbPrefabs.Count - 1); // 기본값은 첫 번째 타입
        Vector2 playerPos = player.position;

        for (int i = 0; i < expSpawnCount; i++)
        {
            // 원형 패턴으로 위치 생성
            float angle = (360f / expSpawnCount) * i;
            float randomDistance = Random.Range(0, maximumSpawnDistance);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
            Vector2 spawnPosition = playerPos + (direction * randomDistance);

            GameObject orb = GetExpOrbFromPool(orbType);
            if (orb != null)
            {
                orb.transform.position = spawnPosition;

                // 일정 시간 후 자동으로 풀로 반환
                StartCoroutine(ReturnOrbAfterDelay(orb, orbType));
            }
        }
    }

    IEnumerator ReturnOrbAfterDelay(GameObject orb, int orbType)
    {
        yield return new WaitForSeconds(10f);

        if (orb.activeInHierarchy)
        {
            ReturnToPool(orb, orbType);
        }
    }

    public void SpawnDropExpOrb(GameObject deadMonster, int monsterDifficulty)
    {
        if (monsterDifficulty >= expOrbPrefabs.Count)
        {
            Debug.LogError($"몬스터 난이도 {monsterDifficulty}가 사용 가능한 경험치 오브 타입을 초과함");
            monsterDifficulty = expOrbPrefabs.Count - 1;
        }

        GameObject orb = GetExpOrbFromPool(monsterDifficulty);
        if (orb != null)
        {
            orb.transform.position = deadMonster.transform.position;
            StartCoroutine(ReturnOrbAfterDelay(orb, monsterDifficulty));
        }
    }

    /// <summary>
    /// 경험치 오브를 플레이어가 수집할 때 풀로 반환하는 메소드
    /// </summary>
    /// <param name="orb"></param>
    /// <param name="orbType"></param>
    public void CollectExpOrb(GameObject orb, int orbType)
    {
        ReturnToPool(orb, orbType);
    }
}