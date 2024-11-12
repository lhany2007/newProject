using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance;

    [Header("Spawn Settings")]
    public GameObject monsterPrefab;
    public Transform Player;
    public float SpawnRadius = 15f; // 스폰 반경
    public int MaxMonsters = 20;
    public float MonsterMaxHP = 100f;
    public int MonstersSpawned = 0; // 현재 게임에 생성되어 있는 몬스터의 수를 추적

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

        // 새로 생성된 몬스터에서 직접 MonsterHealth 컴포넌트를 가져옴
        MonsterHealth monsterHealth = monster.GetComponent<MonsterHealth>();
        if (monsterHealth != null)
        {
            monsterHealth.maxHP = MonsterMaxHP;
            MonstersSpawned++;
        }
        else
        {
            Debug.LogError("몬스터 프리랩에 MonsterHealth 할당 ㄱㄱ");
            Destroy(monster);
        }
    }

    public void OnMonsterDestroyed()
    {
        MonstersSpawned = Mathf.Max(0, MonstersSpawned - 1);
    }
}
