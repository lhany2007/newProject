using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance;

    [Header("Spawn Settings")]
    public List<GameObject> MonsterPrefabList;
    public Transform Player;
    public float SpawnRadius = 15f; // 스폰 반경
    public float MonsterMaxHP = 100f;
    public int MaxMonsters = 100;
    public int MonstersSpawned = 0; // 현재 게임에 생성되어 있는 몬스터의 수를 추적

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
            int chance = random.Next(0, 10); // 0부터 9까지의 숫자를 생성
            monsterTier += (chance < 7) ? 0 : 1; // 30% 확률로 monsterTier++
        }

        GameObject monster = Instantiate(MonsterPrefabList[monsterTier], spawnPosition, Quaternion.identity, spawnParent);
        monster.name = MonsterPrefabList[monsterTier].name;

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
