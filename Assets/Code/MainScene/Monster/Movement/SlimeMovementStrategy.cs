using UnityEngine;
using System.Collections;
using System;

public class SlimeMovementStrategy : IMonsterMovementStrategy
{
    private GameObject monster;
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private MonsterStats.Stats stats;

    private bool isMoving;
    private float waitTime = 1f;
    private float moveDuration = 1f;
    private float speed;

    public void Initialize(GameObject monster, MonsterStats.Stats monsterStats)
    {
        this.monster = monster;
        animator = monster.GetComponent<Animator>();
        rb = monster.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag(Tags.Player)?.transform;

        stats = monsterStats;
        speed = stats.Speed;

        monster.StartCoroutine(MoveRoutine());

        if (player == null)
        {
            throw new NullReferenceException("Player is null");
        }
    }

    public void Move()
    {
        animator.SetBool(AnimationParams.Slime.IsMoving, isMoving);
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - monster.transform.position).normalized;
        monster.transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private IEnumerator MoveRoutine()
    {
        WaitForSeconds waitDelay = new WaitForSeconds(waitTime);
        WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

        while (true)
        {
            // 플레이어를 찾을 때까지 대기
            if (player == null)
            {
                yield return endOfFrame;
                continue;
            }

            isMoving = true;
            float elapsedTime = 0f;
            while (elapsedTime < moveDuration)
            {
                MoveTowardsPlayer();
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            isMoving = false;
            yield return waitDelay;
        }
    }
    
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Player))
        {
            AttackThePlayer();
        }
    }

    private void AttackThePlayer()
    {
        float damage = stats.Damage;
        Vector3 myPos = monster.transform.position;
        bool isKnockback = true;
        MonsterManager.Instance.AttackPlayerOnCollision(damage, myPos, isKnockback);
    }

    public void Stop()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
    }
}
