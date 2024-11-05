using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExpOrbSpawner : MonoBehaviour
{
    public static ExpOrbSpawner Instance;

    [SerializeField] Transform player; // 플레이어 참조
    [SerializeField] List<GameObject> expOrbPrefabs;

    [Header("Spawn Settings")]
    public float baseSpawnRadius = 8f;
    public float spawnVariance = 2f;
    public int spawnBatchSize = 5;
    public float regenerationTime = 2f;
    public int initialPoolSize = 20;

    private const string EXP_TAG = "Exp";
    private Dictionary<int, Queue<GameObject>> expOrbPools;
    private Transform poolContainer;

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
        StartCoroutine(GenerateRandomSpawnLocations());
        TimeManager.Instance.onTierChange.AddListener(OnTierChange);
    }

    // 티어 변경 시 모든 오브를 새 티어로 교체하는 함수
    void OnTierChange(int newTier)
    {
        ReplaceAllOrbsWithNewTier(newTier);
    }

    void ReplaceAllOrbsWithNewTier(int newTier)
    {
        var activeOrbs = GameObject.FindGameObjectsWithTag(EXP_TAG); // 현재 활성화된 오브 리스트를 가져옴

        foreach (var orb in activeOrbs)
        {
            Vector3 oldPosition = orb.transform.position; // 기존 위치 저장

            // 기존 오브를 풀로 반환
            int oldType = int.Parse(orb.name.Split('_')[1]);
            CollectExpOrb(orb, oldType);

            // 새로운 오브 생성
            SpawnExpOrb(oldPosition, newTier);
        }
    }

    // 객체 풀을 초기화하는 함수
    void InitializeObjectPools()
    {
        expOrbPools = new Dictionary<int, Queue<GameObject>>();
        poolContainer = new GameObject("ExpOrbPoolContainer").transform; // 풀 컨테이너 생성

        // 각 타입에 대해 객체 풀 생성
        for (int i = 0; i < expOrbPrefabs.Count; i++)
        {
            var pool = new Queue<GameObject>();

            // 초기 풀 크기만큼 오브를 미리 생성
            for (int j = 0; j < initialPoolSize; j++)
            {
                var newObj = Instantiate(expOrbPrefabs[i], poolContainer);
                newObj.SetActive(false);
                pool.Enqueue(newObj);
            }

            expOrbPools.Add(i, pool);
        }
    }

    // 랜덤 스폰 위치를 생성하는 코루틴
    IEnumerator GenerateRandomSpawnLocations()
    {
        while (true)
        {
            for (int i = 0; i < spawnBatchSize; i++)
            {
                Vector2 spawnPos = Random.insideUnitCircle * baseSpawnRadius + (Vector2)player.position;
                spawnPos += new Vector2(Random.Range(-spawnVariance, spawnVariance),
                                      Random.Range(-spawnVariance, spawnVariance));

                int orbType = Mathf.Min(TimeManager.Instance.currentTier, expOrbPrefabs.Count - 1);

                // 풀에 사용 가능한 오브가 있는지 확인
                if (expOrbPools[orbType].Count > 0)
                {
                    SpawnExpOrb(spawnPos, orbType);
                }
                else
                {
                    // 필요한 경우 풀 확장
                    ExpandPool(orbType);
                    SpawnExpOrb(spawnPos, orbType);
                }
            }

            yield return new WaitForSeconds(regenerationTime);
        }
    }

    // 필요 시 풀 확장하는 새로운 메서드
    void ExpandPool(int orbType)
    {
        var newObj = Instantiate(expOrbPrefabs[orbType], poolContainer);
        newObj.SetActive(false);
        expOrbPools[orbType].Enqueue(newObj);
    }

    // 경험치 오브를 스폰하는 함수
    void SpawnExpOrb(Vector3 position, int orbType)
    {
        // 풀에서 오브를 가져옴
        var orb = expOrbPools[orbType].Dequeue();
        orb.transform.position = position;
        orb.name = $"ExpOrb_{orbType}";
        orb.SetActive(true);
    }

    // 오브를 다시 풀로 반환하는 함수
    public void CollectExpOrb(GameObject orb, int orbType)
    {
        orb.SetActive(false);
        expOrbPools[orbType].Enqueue(orb);
    }

    // 플레이어 근처에서 경험치 오브를 스폰하는 함수
    public void SpawnOrbsNearPlayer(int numOrbs)
    {
        for (int i = 0; i < numOrbs; i++)
        {
            Vector2 spawnPos = Random.insideUnitCircle * baseSpawnRadius + (Vector2)player.position;
            int orbType = Mathf.Min(TimeManager.Instance.currentTier, expOrbPrefabs.Count - 1); // 현재 티어에 맞는 오브 타입 결정
            SpawnExpOrb(spawnPos, orbType);
        }
    }
}
