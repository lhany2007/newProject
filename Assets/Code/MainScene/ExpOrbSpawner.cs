using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExpOrbSpawner : MonoBehaviour
{
    public static ExpOrbSpawner Instance;

    [SerializeField] Transform player;
    [SerializeField] List<GameObject> expOrbPrefabs;

    int expSpawnCount = 3; // �� ���� ������ ����ġ ���� ��
    float regenerationTime = 2f; // ���� ���� �ֱ�
    float maximumSpawnDistance = 5f; // �ִ� ���� �Ÿ�
    int initialPoolSize = 20; // �ʱ� Ǯ ũ��

    // �� ����ġ ���� Ÿ�Կ� ���� ��ü Ǯ
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
        // ����ġ ���긦 �ֱ������� ����
        StartCoroutine(GenerateRandomSpawnLocations());
    }

    /// <summary>
    /// ������ �̸� �ν��Ͻ�ȭ
    /// </summary>
    void InitializeObjectPools()
    {
        // �� Ÿ���� ����ġ ���긦 ���� ��ü Ǯ �ʱ�ȭ
        expOrbPools = new Dictionary<int, Queue<GameObject>>();
        poolContainer = new GameObject("ExpOrbPool").transform;
        poolContainer.SetParent(transform);

        for (int i = 0; i < expOrbPrefabs.Count; i++)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int j = 0; j < initialPoolSize; j++)
            {
                CreateNewExpOrb(i, pool); // �� ���� ����
            }
            expOrbPools[i] = pool; 
        }
    }

    void CreateNewExpOrb(int orbType, Queue<GameObject> pool)
    {
        if (orbType >= expOrbPrefabs.Count)
        {
            Debug.LogError($"�߸��� ���� Ÿ��: {orbType}. �ִ� Ÿ���� {expOrbPrefabs.Count - 1}");
            return;
        }

        GameObject orb = Instantiate(expOrbPrefabs[orbType], poolContainer); // �ν��Ͻ�ȭ 
        orb.SetActive(false);
        pool.Enqueue(orb);
    }

    GameObject GetExpOrbFromPool(int orbType)
    {
        if (!expOrbPools.TryGetValue(orbType, out Queue<GameObject> pool))
        {
            Debug.LogError($"���� Ÿ�� {orbType}�� ���� Ǯ�� ����");
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
    /// ����� ���� ���긦 ��Ȱ��ȭ�ϰ� Ǯ�� ��ȯ
    /// </summary>
    void ReturnToPool(GameObject orb, int orbType)
    {
        orb.SetActive(false);
        expOrbPools[orbType].Enqueue(orb);
    }

    /// <summary>
    /// �ֱ������� ���� ��ġ�� ����ġ ���� ����
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
        int orbType = Mathf.Min(0, expOrbPrefabs.Count - 1); // �⺻���� ù ��° Ÿ��
        Vector2 playerPos = player.position;

        for (int i = 0; i < expSpawnCount; i++)
        {
            // ���� �������� ��ġ ����
            float angle = (360f / expSpawnCount) * i;
            float randomDistance = Random.Range(0, maximumSpawnDistance);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
            Vector2 spawnPosition = playerPos + (direction * randomDistance);

            GameObject orb = GetExpOrbFromPool(orbType);
            if (orb != null)
            {
                orb.transform.position = spawnPosition;

                // ���� �ð� �� �ڵ����� Ǯ�� ��ȯ
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
            Debug.LogError($"���� ���̵� {monsterDifficulty}�� ��� ������ ����ġ ���� Ÿ���� �ʰ���");
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
    /// ����ġ ���긦 �÷��̾ ������ �� Ǯ�� ��ȯ�ϴ� �޼ҵ�
    /// </summary>
    /// <param name="orb"></param>
    /// <param name="orbType"></param>
    public void CollectExpOrb(GameObject orb, int orbType)
    {
        ReturnToPool(orb, orbType);
    }
}