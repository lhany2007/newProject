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

    // 변수 초기화 함수
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
            MonsterDictionary[i] = new Dictionary<string, GameObject>(); // 각 인덱스 초기화
            MonsterDictionary[i].Add(monsterNameList[i], monsterPrefabList[i]);
        }
    }

    // 예외처리 함수
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
            Debug.LogError("List의 크기가 다름");
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
        Debug.LogError($"존재하지 않는 몬스터 이름: {name}");
        return -1;
    }
}

public class MonsterStats
{
    public Tuple<float, float, float>[] MonsterStatsTuple;

    private List<float> monsterMaxHPList;
    private List<float> monsterSpeedList;
    private List<float> monsterDamageList;

    public Stats stats; // stats 구조체 인스턴스

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

        // stats 구조체 초기화
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
                throw new IndexOutOfRangeException("인덱스 범위 초과");
            }
            return stats[index].Item1; // Max HP
        }

        public float Speed(int index)
        {
            if (index < 0 || index >= stats.Length)
            {
                throw new IndexOutOfRangeException("인덱스 범위 초과");
            }
            return stats[index].Item2; // Speed
        }

        public float Damage(int index)
        {
            if (index < 0 || index >= stats.Length)
            {
                throw new IndexOutOfRangeException("인덱스 범위 초과");
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
    /// 플레이어를 기준으로 몬스터의 방향 조정
    /// </summary>
    public Vector3 UpdateDirection(Transform transform)
    {
        Vector2 direction = (TargetPlayer.position - transform.position).normalized;
        float scaleX = direction.x > 0 ? 1f : -1f;
        return new Vector3(scaleX, 1f, 1f);
    }
}