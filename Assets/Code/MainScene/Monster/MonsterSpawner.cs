using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance;

    [Header("Spawn Settings")]
    public GameObject monsterPrefab;
    public Transform Player;
    public float SpawnRadius = 15f; // ���� �ݰ�
    public int MaxMonsters = 20;
    public float MonsterMaxHP = 100f;
    public int MonstersSpawned = 0; // ���� ���ӿ� �����Ǿ� �ִ� ������ ���� ����

    Transform spawnParent;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        spawnParent = transform;
    }

    public void SpawnMonster()
    {
        Vector3 randomOffset = Random.insideUnitCircle * SpawnRadius;
        Vector3 spawnPosition = Player.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

        GameObject monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity, spawnParent);

        // ���� ������ ���Ϳ��� ���� MonsterHealth ������Ʈ�� ������
        MonsterHealth monsterHealth = monster.GetComponent<MonsterHealth>();
        if (monsterHealth != null)
        {
            monsterHealth.maxHP = MonsterMaxHP;
            MonstersSpawned++;
        }
        else
        {
            Debug.LogError("���� �������� MonsterHealth �Ҵ� ����");
            Destroy(monster);
        }
    }

    public void OnMonsterDestroyed()
    {
        MonstersSpawned = Mathf.Max(0, MonstersSpawned - 1);
    }
}
