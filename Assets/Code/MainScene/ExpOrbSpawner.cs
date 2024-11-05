using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExpOrbSpawner : MonoBehaviour
{
    public static ExpOrbSpawner Instance;

    [SerializeField] Transform player;
    [SerializeField] List<GameObject> expOrbPrefabList;

    [Header("Spawn Settings")]
    public float BaseSpawnRadius = 8f; // 기본 생성 반경
    public float SpawnVariance = 2f; // 생성 위치 변동성 (랜덤 위치 오차)
    public int SpawnBatchSize = 5; // 한 번에 생성할 오브 개수
    public float RegenerationTime = 2f; // 새로운 오브 생성 대기 시간
    public int InitialPoolSize = 20; // 초기 풀의 오브 개수
    public int ExpandPoolSize = 5; // 풀을 확장할 때 추가할 오브 개수

    const string EXP_TAG = "Exp";

    Dictionary<int, Queue<GameObject>> expOrbPools; // 각 티어별 오브 객체 풀
    Transform poolContainer; // 풀에 있는 오브들을 관리하는 부모 객체

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
        StartCoroutine(GenerateRandomSpawnLocations()); // 정기적인 오브 생성 시작
        TimeManager.Instance.onTierChange.AddListener(OnTierChange); // Tier 변화에 따른 콜백 등록
    }

    void OnTierChange(int newTier)
    {
        // 티어 변경 시 모든 기존 오브를 새로운 티어 오브로 교체
        ReplaceAllOrbsWithNewTier(newTier);
    }

    void ReplaceAllOrbsWithNewTier(int newTier)
    {
        // 현재 활성화된 모든 경험치 오브 수집 후 새로운 티어로 생성
        var activeOrbs = GameObject.FindGameObjectsWithTag(EXP_TAG);

        foreach (var orb in activeOrbs)
        {
            int oldType = GetOrbTypeFromName(orb.name);
            if (oldType >= 0)
            {
                CollectExpOrb(orb, oldType); // 기존 오브를 풀로 반환
            }
            else
            {
                Destroy(orb); // 해당하지 않는 경우 삭제
            }
        }

        // 새 티어에 맞는 오브를 설정된 개수만큼 생성
        for (int i = 0; i < SpawnBatchSize; i++)
        {
            Vector2 spawnPos = Random.insideUnitCircle * BaseSpawnRadius + (Vector2)player.position;
            spawnPos += new Vector2(Random.Range(-SpawnVariance, SpawnVariance),
                                  Random.Range(-SpawnVariance, SpawnVariance));

            SpawnExpOrb(spawnPos, newTier);
        }
    }

    int GetOrbTypeFromName(string orbName)
    {
        // 오브 이름에서 티어(타입)를 파싱하여 반환
        orbName = orbName.Replace("(Clone)", "").Trim();

        string[] nameParts = orbName.Split('_');
        if (nameParts.Length >= 2 && int.TryParse(nameParts[1], out int type))
        {
            return Mathf.Clamp(type, 0, expOrbPrefabList.Count - 1);
        }
        return -1;
    }

    void InitializeObjectPools()
    {
        // 각 티어별로 오브젝트 풀을 초기화하고 설정된 초기 개수만큼 오브 생성
        expOrbPools = new Dictionary<int, Queue<GameObject>>();
        poolContainer = new GameObject("ExpOrbPoolContainer").transform;
        poolContainer.SetParent(transform);

        for (int i = 0; i < expOrbPrefabList.Count; i++)
        {
            var pool = new Queue<GameObject>();
            CreateNewPoolObjects(i, InitialPoolSize, pool);
            expOrbPools.Add(i, pool);
        }
    }

    IEnumerator GenerateRandomSpawnLocations()
    {
        while (true)
        {
            // 지정된 배치 크기만큼 주기적으로 오브 생성
            for (int i = 0; i < SpawnBatchSize; i++)
            {
                Vector2 spawnPos = Random.insideUnitCircle * BaseSpawnRadius + (Vector2)player.position;
                spawnPos += new Vector2(Random.Range(-SpawnVariance, SpawnVariance),
                                      Random.Range(-SpawnVariance, SpawnVariance));

                int orbType = Mathf.Min(TimeManager.Instance.CurrentTier, expOrbPrefabList.Count - 1);

                if (expOrbPools[orbType].Count > 0)
                {
                    SpawnExpOrb(spawnPos, orbType);
                }
                else
                {
                    ExpandPool(orbType); // 풀에 오브가 없을 경우 풀 확장
                    SpawnExpOrb(spawnPos, orbType);
                }
            }

            yield return new WaitForSeconds(RegenerationTime);
        }
    }

    void ExpandPool(int orbType)
    {
        // 풀을 확장하고 새 오브를 풀에 추가
        var newObj = Instantiate(expOrbPrefabList[orbType], poolContainer);
        newObj.name = $"ExpOrb_{orbType}";
        newObj.SetActive(false);
        expOrbPools[orbType].Enqueue(newObj);
    }

    void SpawnExpOrb(Vector3 position, int orbType)
    {
        // 오브를 활성화하고 지정된 위치에 배치
        int clampedType = Mathf.Clamp(orbType, 0, expOrbPrefabList.Count - 1);

        var pool = expOrbPools[clampedType];

        if (pool.Count > 0)
        {
            var orb = pool.Dequeue();
            orb.transform.position = position;
            orb.SetActive(true);
        }
    }

    void CreateNewPoolObjects(int orbType, int count, Queue<GameObject> pool)
    {
        // 오브 풀에 새 오브를 생성하고 추가
        for (int j = 0; j < count; j++)
        {
            var newObj = Instantiate(expOrbPrefabList[orbType], poolContainer);
            newObj.name = $"ExpOrb_{orbType}";
            newObj.SetActive(false);
            pool.Enqueue(newObj);
        }
    }

    public void CollectExpOrb(GameObject orb, int orbType)
    {
        // 오브를 수집하여 비활성화하고 풀로 반환
        int clampedType = Mathf.Clamp(orbType, 0, expOrbPrefabList.Count - 1);

        orb.SetActive(false);
        expOrbPools[clampedType].Enqueue(orb);
    }

    public void SpawnOrbsNearPlayer(int numOrbs)
    {
        // 지정된 개수만큼 플레이어 주변에 오브 생성
        for (int i = 0; i < numOrbs; i++)
        {
            Vector2 spawnPos = Random.insideUnitCircle * BaseSpawnRadius + (Vector2)player.position;
            int orbType = Mathf.Min(TimeManager.Instance.CurrentTier, expOrbPrefabList.Count - 1);
            SpawnExpOrb(spawnPos, orbType);
        }
    }
}