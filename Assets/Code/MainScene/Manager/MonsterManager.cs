using System;
using UnityEngine;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour
{
    private static MonsterManager _instance;
    public static MonsterManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<MonsterManager>();
                if (_instance == null)
                {
                    throw new Exception("MonsterManager instance is null");
                }
            }
            return _instance;
        }
    }

    public Dictionary<string, GameObject>[] MonsterDictionary;

    public int MonsterCount = 0;

    [SerializeField] private List<GameObject> monsterPrefabList;
    private List<string> monsterNameList;

    private const int MONSTER_PREFAB_COUNT = 2;

    public MonsterStats monsterStats { get; private set; }
    public MonsterMovement monsterMovement { get; private set; }

    private void Awake()
    {
        InitializeVariable();
        ExceptionHandling();
    }

    // ���� �ʱ�ȭ �Լ�
    private void InitializeVariable()
    {
        _instance = this;

        monsterStats = new MonsterStats();
        monsterMovement = new MonsterMovement();

        monsterNameList = new List<string> { "Slime", "Bat" };
        MonsterDictionary = new Dictionary<string, GameObject>[MONSTER_PREFAB_COUNT];
        monsterStats.InitializeStats(MONSTER_PREFAB_COUNT);
        monsterMovement.TargetPlayer = GameObject.FindGameObjectWithTag(Tags.PLAYER).transform;
        MonsterAddition();
    }
    private void MonsterAddition()
    {
        for (int i = 0; i < MONSTER_PREFAB_COUNT; i++)
        {
            MonsterDictionary[i] = new Dictionary<string, GameObject>(); // �� �ε��� �ʱ�ȭ
            MonsterDictionary[i].Add(monsterNameList[i], monsterPrefabList[i]);
        }
    }

    // ����ó�� �Լ�
    private void ExceptionHandling()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        if (monsterPrefabList == null || monsterPrefabList.Count == 0)
        {
            throw new NullReferenceException("monsterPrefabList is null");
        }
        if (monsterPrefabList.Count != monsterNameList.Count)
        {
            Debug.LogError("List�� ũ�Ⱑ �ٸ�");
        }
        if (monsterMovement.TargetPlayer == null)
        {
            throw new NullReferenceException("TargetPlayer is null");
        }
    }

    public int GetMonsterNameIndex(string name)
    {
        for (int i = 0; i < MONSTER_PREFAB_COUNT; i++)
        {
            if (MonsterDictionary[i].ContainsKey(name))
            {
                return i;
            }
        }
        Debug.LogError($"�������� �ʴ� ���� �̸�: {name}");
        return -1;
    }
}

public class MonsterStats
{
    public Tuple<float, float, float>[] MonsterStatsTuple;

    private List<float> monsterMaxHPList;
    private List<float> monsterSpeedList;
    private List<float> monsterDamageList;

    public Stats stats; // stats ����ü �ν��Ͻ�

    public void InitializeStats(int monsterCount)
    {
        monsterMaxHPList = new List<float> { 100, 150 };
        monsterSpeedList = new List<float> { 3, 2 };
        monsterDamageList = new List<float> { 10, 15 };

        MonsterStatsTuple = new Tuple<float, float, float>[monsterCount];

        for (int i = 0; i < monsterCount; i++)
        {
            MonsterStatsTuple[i] = new Tuple<float, float, float>(monsterMaxHPList[i], monsterSpeedList[i], monsterDamageList[i]);
        }

        // stats ����ü �ʱ�ȭ
        stats = new Stats(MonsterStatsTuple);
    }

    public struct Stats
    {
        private readonly Tuple<float, float, float>[] stats;

        public Stats(Tuple<float, float, float>[] _stats)
        {
            stats = _stats;
        }

        public float HP(int index)
        {
            if (index < 0 || index >= stats.Length)
            {
                throw new IndexOutOfRangeException("�ε��� ���� �ʰ�");
            }
            return stats[index].Item1; // Max HP
        }

        public float Speed(int index)
        {
            if (index < 0 || index >= stats.Length)
            {
                throw new IndexOutOfRangeException("�ε��� ���� �ʰ�");
            }
            return stats[index].Item2; // Speed
        }

        public float Damage(int index)
        {
            if (index < 0 || index >= stats.Length)
            {
                throw new IndexOutOfRangeException("�ε��� ���� �ʰ�");
            }
            return stats[index].Item3; // Damage
        }
    }
}

public class MonsterMovement
{
    public Transform TargetPlayer;

    public Vector3 MoveToPlayer(float speed, Transform transform)
    {
        return Vector2.MoveTowards(transform.position, TargetPlayer.position, speed * Time.deltaTime);
    }

    /// <summary>
    /// �÷��̾ �������� ������ ���� ����
    /// </summary>
    public Vector3 UpdateDirection(Transform transform)
    {
        Vector2 direction = (TargetPlayer.position - transform.position).normalized;
        float scaleX = direction.x > 0 ? 1f : -1f;
        return new Vector3(scaleX, 1f, 1f);
    }
}