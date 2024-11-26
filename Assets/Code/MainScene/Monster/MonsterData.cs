using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Game/Monster Data")]
public class MonsterData : ScriptableObject
{
    [System.Serializable]
    public class MonsterInfo
    {
        public string name;
        public GameObject prefab;
        public int tier;
        public float maxHP;
        public float speed;
        public float damage;
    }

    public List<MonsterInfo> monsters;
    private Dictionary<string, MonsterInfo> monsterDictionary;

    // ScriptableObject가 활성화될 때 호출
    private void OnEnable()
    {
        InitializeDictionary();
    }

    /// <summary>
    /// 딕셔너리 초기화
    /// </summary>
    private void InitializeDictionary()
    {
        monsterDictionary = new Dictionary<string, MonsterInfo>();

        foreach (var monster in monsters)
        {
            monsterDictionary[monster.name] = monster; 
        }
    }

    // 몬스터 스탯 반환
    public MonsterStats.Stats GetMonsterStats(string monsterName)
    {
        var info = GetMonsterInfo(monsterName);
        return new MonsterStats.Stats(info.maxHP, info.speed, info.damage);
    }

    /// <summary>
    /// 몬스터 프리팹 반환
    /// </summary>
    public GameObject GetMonsterPrefab(string monsterName)
    {
        return GetMonsterInfo(monsterName).prefab;
    }

    // 몬스터 이름 반환
    public string GetMonsterName(int monsterTier)
    {
        return GetMonsterInfo(monsterTier).name;
    }

    /// <summary>
    /// 몬스터 이름에 해당하는 정보를 반환
    /// </summary>
    /// <returns>몬스터 정보(MonsterInfo)</returns>
    private MonsterInfo GetMonsterInfo(string monsterName)
    {
        if (!monsterDictionary.TryGetValue(monsterName, out MonsterInfo info))
        {
            throw new ArgumentException($"존재하지 않는 Monster이름: {monsterName}");
        }
        return info;
    }

    /// <summary>
    /// 몬스터 티어에 해당하는 정보를 반환
    /// </summary>
    /// <returns>몬스터 정보(MonsterInfo)</returns>
    private MonsterInfo GetMonsterInfo(int tier)
    {
        // var monsterInfo = monsters.FirstOrDefault(m => m.tier == tier);
        return monsters.FirstOrDefault(m => m.tier == tier) ?? throw new ArgumentException($"존재하지 않는 Monster티어: {tier}");
    }
}