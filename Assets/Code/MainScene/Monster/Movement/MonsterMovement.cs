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
        InitializeComponents(); // 컴포넌트 초기화
        InitializeStrategy();   // 몬스터 이동 로직 설정
    }

    protected virtual void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(Tags.Player)?.transform;

        // 몬스터의 스탯 데이터를 가져옴
        if (!TryGetMonsterStats(out stats))
        {
            enabled = false; // 가져오지 못하면 스크립트가 동작하지 않도록 비활성화
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
                throw new NullReferenceException("존재하지 않는 몬스터 태그");
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
            Debug.LogError($"{gameObject.tag}몬스터의 스탯을 가져오는데 실패함");
            monsterStats = new MonsterStats.Stats(0, 0, 0);
            return false;
        }
    }
}