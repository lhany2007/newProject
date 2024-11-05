using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExpOrbSpawner : MonoBehaviour
{
    public static ExpOrbSpawner Instance;

    [SerializeField] Transform player; // �÷��̾� ����
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

    // Ƽ�� ���� �� ��� ���긦 �� Ƽ��� ��ü�ϴ� �Լ�
    void OnTierChange(int newTier)
    {
        ReplaceAllOrbsWithNewTier(newTier);
    }

    void ReplaceAllOrbsWithNewTier(int newTier)
    {
        var activeOrbs = GameObject.FindGameObjectsWithTag(EXP_TAG); // ���� Ȱ��ȭ�� ���� ����Ʈ�� ������

        foreach (var orb in activeOrbs)
        {
            Vector3 oldPosition = orb.transform.position; // ���� ��ġ ����

            // ���� ���긦 Ǯ�� ��ȯ
            int oldType = int.Parse(orb.name.Split('_')[1]);
            CollectExpOrb(orb, oldType);

            // ���ο� ���� ����
            SpawnExpOrb(oldPosition, newTier);
        }
    }

    // ��ü Ǯ�� �ʱ�ȭ�ϴ� �Լ�
    void InitializeObjectPools()
    {
        expOrbPools = new Dictionary<int, Queue<GameObject>>();
        poolContainer = new GameObject("ExpOrbPoolContainer").transform; // Ǯ �����̳� ����

        // �� Ÿ�Կ� ���� ��ü Ǯ ����
        for (int i = 0; i < expOrbPrefabs.Count; i++)
        {
            var pool = new Queue<GameObject>();

            // �ʱ� Ǯ ũ�⸸ŭ ���긦 �̸� ����
            for (int j = 0; j < initialPoolSize; j++)
            {
                var newObj = Instantiate(expOrbPrefabs[i], poolContainer);
                newObj.SetActive(false);
                pool.Enqueue(newObj);
            }

            expOrbPools.Add(i, pool);
        }
    }

    // ���� ���� ��ġ�� �����ϴ� �ڷ�ƾ
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

                // Ǯ�� ��� ������ ���갡 �ִ��� Ȯ��
                if (expOrbPools[orbType].Count > 0)
                {
                    SpawnExpOrb(spawnPos, orbType);
                }
                else
                {
                    // �ʿ��� ��� Ǯ Ȯ��
                    ExpandPool(orbType);
                    SpawnExpOrb(spawnPos, orbType);
                }
            }

            yield return new WaitForSeconds(regenerationTime);
        }
    }

    // �ʿ� �� Ǯ Ȯ���ϴ� ���ο� �޼���
    void ExpandPool(int orbType)
    {
        var newObj = Instantiate(expOrbPrefabs[orbType], poolContainer);
        newObj.SetActive(false);
        expOrbPools[orbType].Enqueue(newObj);
    }

    // ����ġ ���긦 �����ϴ� �Լ�
    void SpawnExpOrb(Vector3 position, int orbType)
    {
        // Ǯ���� ���긦 ������
        var orb = expOrbPools[orbType].Dequeue();
        orb.transform.position = position;
        orb.name = $"ExpOrb_{orbType}";
        orb.SetActive(true);
    }

    // ���긦 �ٽ� Ǯ�� ��ȯ�ϴ� �Լ�
    public void CollectExpOrb(GameObject orb, int orbType)
    {
        orb.SetActive(false);
        expOrbPools[orbType].Enqueue(orb);
    }

    // �÷��̾� ��ó���� ����ġ ���긦 �����ϴ� �Լ�
    public void SpawnOrbsNearPlayer(int numOrbs)
    {
        for (int i = 0; i < numOrbs; i++)
        {
            Vector2 spawnPos = Random.insideUnitCircle * baseSpawnRadius + (Vector2)player.position;
            int orbType = Mathf.Min(TimeManager.Instance.currentTier, expOrbPrefabs.Count - 1); // ���� Ƽ� �´� ���� Ÿ�� ����
            SpawnExpOrb(spawnPos, orbType);
        }
    }
}
