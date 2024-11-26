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

    // ScriptableObject�� Ȱ��ȭ�� �� ȣ��
    private void OnEnable()
    {
        InitializeDictionary();
    }

    /// <summary>
    /// ��ųʸ� �ʱ�ȭ
    /// </summary>
    private void InitializeDictionary()
    {
        monsterDictionary = new Dictionary<string, MonsterInfo>();

        foreach (var monster in monsters)
        {
            monsterDictionary[monster.name] = monster; 
        }
    }

    // ���� ���� ��ȯ
    public MonsterStats.Stats GetMonsterStats(string monsterName)
    {
        var info = GetMonsterInfo(monsterName);
        return new MonsterStats.Stats(info.maxHP, info.speed, info.damage);
    }

    /// <summary>
    /// ���� ������ ��ȯ
    /// </summary>
    public GameObject GetMonsterPrefab(string monsterName)
    {
        return GetMonsterInfo(monsterName).prefab;
    }

    // ���� �̸� ��ȯ
    public string GetMonsterName(int monsterTier)
    {
        return GetMonsterInfo(monsterTier).name;
    }

    /// <summary>
    /// ���� �̸��� �ش��ϴ� ������ ��ȯ
    /// </summary>
    /// <returns>���� ����(MonsterInfo)</returns>
    private MonsterInfo GetMonsterInfo(string monsterName)
    {
        if (!monsterDictionary.TryGetValue(monsterName, out MonsterInfo info))
        {
            throw new ArgumentException($"�������� �ʴ� Monster�̸�: {monsterName}");
        }
        return info;
    }

    /// <summary>
    /// ���� Ƽ� �ش��ϴ� ������ ��ȯ
    /// </summary>
    /// <returns>���� ����(MonsterInfo)</returns>
    private MonsterInfo GetMonsterInfo(int tier)
    {
        // var monsterInfo = monsters.FirstOrDefault(m => m.tier == tier);
        return monsters.FirstOrDefault(m => m.tier == tier) ?? throw new ArgumentException($"�������� �ʴ� MonsterƼ��: {tier}");
    }
}