using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExpOrbSpawner : MonoBehaviour
{
    public static ExpOrbSpawner Instance;

    [SerializeField] Transform player;
    [SerializeField] List<GameObject> expOrbPrefabList;

    [Header("Spawn Settings")]
    public float BaseSpawnRadius = 8f; // �⺻ ���� �ݰ�
    public float SpawnVariance = 2f; // ���� ��ġ ������ (���� ��ġ ����)
    public int SpawnBatchSize = 5; // �� ���� ������ ���� ����
    public float RegenerationTime = 2f; // ���ο� ���� ���� ��� �ð�
    public int InitialPoolSize = 20; // �ʱ� Ǯ�� ���� ����
    public int ExpandPoolSize = 5; // Ǯ�� Ȯ���� �� �߰��� ���� ����

    const string EXP_TAG = "Exp";

    Dictionary<int, Queue<GameObject>> expOrbPools; // �� Ƽ� ���� ��ü Ǯ
    Transform poolContainer; // Ǯ�� �ִ� ������� �����ϴ� �θ� ��ü

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
        StartCoroutine(GenerateRandomSpawnLocations()); // �������� ���� ���� ����
        TimeManager.Instance.onTierChange.AddListener(OnTierChange); // Tier ��ȭ�� ���� �ݹ� ���
    }

    void OnTierChange(int newTier)
    {
        // Ƽ�� ���� �� ��� ���� ���긦 ���ο� Ƽ�� ����� ��ü
        ReplaceAllOrbsWithNewTier(newTier);
    }

    void ReplaceAllOrbsWithNewTier(int newTier)
    {
        // ���� Ȱ��ȭ�� ��� ����ġ ���� ���� �� ���ο� Ƽ��� ����
        var activeOrbs = GameObject.FindGameObjectsWithTag(EXP_TAG);

        foreach (var orb in activeOrbs)
        {
            int oldType = GetOrbTypeFromName(orb.name);
            if (oldType >= 0)
            {
                CollectExpOrb(orb, oldType); // ���� ���긦 Ǯ�� ��ȯ
            }
            else
            {
                Destroy(orb); // �ش����� �ʴ� ��� ����
            }
        }

        // �� Ƽ� �´� ���긦 ������ ������ŭ ����
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
        // ���� �̸����� Ƽ��(Ÿ��)�� �Ľ��Ͽ� ��ȯ
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
        // �� Ƽ��� ������Ʈ Ǯ�� �ʱ�ȭ�ϰ� ������ �ʱ� ������ŭ ���� ����
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
            // ������ ��ġ ũ�⸸ŭ �ֱ������� ���� ����
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
                    ExpandPool(orbType); // Ǯ�� ���갡 ���� ��� Ǯ Ȯ��
                    SpawnExpOrb(spawnPos, orbType);
                }
            }

            yield return new WaitForSeconds(RegenerationTime);
        }
    }

    void ExpandPool(int orbType)
    {
        // Ǯ�� Ȯ���ϰ� �� ���긦 Ǯ�� �߰�
        var newObj = Instantiate(expOrbPrefabList[orbType], poolContainer);
        newObj.name = $"ExpOrb_{orbType}";
        newObj.SetActive(false);
        expOrbPools[orbType].Enqueue(newObj);
    }

    void SpawnExpOrb(Vector3 position, int orbType)
    {
        // ���긦 Ȱ��ȭ�ϰ� ������ ��ġ�� ��ġ
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
        // ���� Ǯ�� �� ���긦 �����ϰ� �߰�
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
        // ���긦 �����Ͽ� ��Ȱ��ȭ�ϰ� Ǯ�� ��ȯ
        int clampedType = Mathf.Clamp(orbType, 0, expOrbPrefabList.Count - 1);

        orb.SetActive(false);
        expOrbPools[clampedType].Enqueue(orb);
    }

    public void SpawnOrbsNearPlayer(int numOrbs)
    {
        // ������ ������ŭ �÷��̾� �ֺ��� ���� ����
        for (int i = 0; i < numOrbs; i++)
        {
            Vector2 spawnPos = Random.insideUnitCircle * BaseSpawnRadius + (Vector2)player.position;
            int orbType = Mathf.Min(TimeManager.Instance.CurrentTier, expOrbPrefabList.Count - 1);
            SpawnExpOrb(spawnPos, orbType);
        }
    }
}