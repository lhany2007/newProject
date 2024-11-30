using System;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    protected IMonsterMovementStrategy movementStrategy;

    protected MonsterStats.Stats stats;

    protected Transform player;
    protected Rigidbody2D rb;
    protected Animator animator;

    protected virtual void Start()
    {
        InitializeComponents(); // ������Ʈ �ʱ�ȭ
        InitializeStrategy();   // ���� �̵� ���� ����
    }

    protected virtual void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(Tags.Player)?.transform;

        // ������ ���� �����͸� ������
        if (!TryGetMonsterStats(out stats))
        {
            enabled = false; // �������� ���ϸ� ��ũ��Ʈ�� �������� �ʵ��� ��Ȱ��ȭ
            return;
        }
    }

    protected virtual void InitializeStrategy()
    {
        switch (gameObject.tag)
        {
            case "Bat":
                movementStrategy = new BatMovementStrategy();
                break;
            case "Slime":
                movementStrategy = new SlimeMovementStrategy();
                break;
            default:
                throw new NullReferenceException("�������� �ʴ� ���� �±�");
        }

        movementStrategy.Initialize(gameObject, stats);
    }

    protected virtual void Update()
    {
        movementStrategy?.Move();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        movementStrategy?.OnCollisionEnter2D(collision);
    }

    protected virtual bool TryGetMonsterStats(out MonsterStats.Stats monsterStats)
    {
        try
        {
            monsterStats = MonsterSpawner.Instance.monsterData.GetMonsterStats(gameObject.tag);
            return true;
        }
        catch (System.ArgumentException)
        {
            Debug.LogError($"{gameObject.tag}������ ������ �������µ� ������");
            monsterStats = new MonsterStats.Stats(0, 0, 0);
            return false;
        }
    }
}