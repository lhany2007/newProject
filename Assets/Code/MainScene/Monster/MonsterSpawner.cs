using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    private static MonsterSpawner _instance;
    public static MonsterSpawner Instance
    {
        get => _instance;
        private set => _instance = value;
    }

    [Header("Spawn Settings")]
    [SerializeField] private Transform player; // �÷��̾� Transform ����
    [SerializeField] private float spawnRadius = 15f; // �÷��̾� �ֺ� ���� �ݰ�
    public int MaxMonsters = 300; // �ִ� ���� ������ ���� ��

    public int MonstersSpawned = 0; // ���� ������ ���� ��

    private readonly System.Random random = new();
    private Transform spawnParent; // ������ ������ �θ� Transform

    public MonsterData monsterData; // ���� ������ ScriptableObject ����

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        spawnParent = transform; // ������ ���͵��� �θ� ����
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag(Tags.Player).transform; // �÷��̾ �±׷� ã��
        }
        if (player == null)
        {
            throw new System.NullReferenceException("Player is null");
        }
    }

    // �÷��̾� �ֺ��� ������ ��ġ�� ���͸� ����
    public GameObject SpawnMonster()
    {
        // �ִ� ���� ���� �ʰ��ϸ� ���� �Ұ�
        if (MonstersSpawned >= MaxMonsters)
        {
            return null;
        }

        int monsterTier = CalculateMonsterTier(); // ���� Ƽ�� ���
        string monsterName = monsterData.GetMonsterName(monsterTier); // Ƽ� �ش��ϴ� ���� �̸� ��������
        Vector3 position = GetRandomSpawnPosition(); // ���� ��ġ ���

        GameObject monsterPrefab = monsterData.GetMonsterPrefab(monsterName);

        // ���� �ν��Ͻ�ȭ �� �ʱ�ȭ
        GameObject monster = Instantiate(monsterPrefab, position, Quaternion.identity, spawnParent);
        InitializeMonster(monster, monsterTier);
        return monster;
    }

    // �÷��̾� �ֺ� ������ ��ġ ���
    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomOffset = Random.insideUnitCircle * spawnRadius; // ������ ���� ��ġ ���
        return player.position + new Vector3(randomOffset.x, randomOffset.y, 0f); // �÷��̾� ��ġ �������� ������ ����
    }

    // ���� �ð� �� Ȯ���� ���� ���� Ƽ�� ���
    private int CalculateMonsterTier()
    {
        int monsterTier = TimeManager.Instance.MonsterTier; // ���� �ð��� ���� �⺻ Ƽ��

        int chance = random.Next(0, 10); // 0~9 ������ ���� ����
        monsterTier += (chance < 7) ? 0 : 1; // 30% Ȯ���� Ƽ�� ����

        return monsterTier;
    }

    // ������ ���͸� �ʱ�ȭ
    private void InitializeMonster(GameObject monster, int tier)
    {
        var monsterHealth = monster.GetComponent<MonsterHealth>();
        if (monsterHealth == null)
        {
            throw new System.NullReferenceException("monsterHealth is null");
        }
        monster.name = monsterData.GetMonsterName(tier);// ���� �̸� ����
        var stats = monsterData.GetMonsterStats(monster.name);
        monsterHealth.Initialize(stats.MaxHP); // ü�� �ʱ�ȭ
        MonstersSpawned++;
    }

    // ���Ͱ� �ı��� �� ȣ��Ǿ� ī���� ����
    public void OnMonsterDestroyed()
    {
        MonstersSpawned = Mathf.Max(0, MonstersSpawned - 1); // �ּҰ� 0���� ����
    }
}
