using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance;

    [Header("Spawn Settings")]
    public List<GameObject> MonsterPrefabList;
    public Transform Player;
    public float SpawnRadius = 15f; // ���� �ݰ�
    public float MonsterMaxHP = 100f;
    public int MaxMonsters = 100;
    public int MonstersSpawned = 0; // ���� ���ӿ� �����Ǿ� �ִ� ������ ���� ����

    readonly System.Random random = new();
    
    Transform spawnParent;
    public Dictionary<string, int> MonsterDamageDictionary;
    List<int> monsterDamageList;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        spawnParent = transform;

        MonsterDamageDictionary = new Dictionary<string, int>();
        monsterDamageList = new List<int> { 10, 15, 20, 25, 30 };
        AddMonsterDamage();
    }

    public void AddMonsterDamage()
    {
        int index = 0;
        foreach (var item in MonsterPrefabList)
        {
            MonsterDamageDictionary.Add(item.name, monsterDamageList[index]);
            index++;
        }
    }

    public void SpawnMonster()
    {
        Vector3 randomOffset = Random.insideUnitCircle * SpawnRadius;
        Vector3 spawnPosition = Player.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

        int monsterTier = TimeManager.Instance.MonsterTier;
        if (monsterTier != MonsterPrefabList.Count)
        {
            int chance = random.Next(0, 10); // 0���� 9������ ���ڸ� ����
            monsterTier += (chance < 7) ? 0 : 1; // 30% Ȯ���� monsterTier++
        }

        GameObject monster = Instantiate(MonsterPrefabList[monsterTier], spawnPosition, Quaternion.identity, spawnParent);
        monster.name = MonsterPrefabList[monsterTier].name;

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
