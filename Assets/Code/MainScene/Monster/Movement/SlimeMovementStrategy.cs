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
        player = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player.ToString())?.transform;

        stats = monsterStats;
        speed = stats.Speed;

        monster.StartCoroutine(MoveRoutine());
    }

    public void Move()
    {
        animator.SetBool(GameConstants.AnimationParams.IsMoving.ToString(), isMoving);
    }

    private void MoveTowardsPlayer()
    {
        if (player == null)
        {
            throw new NullReferenceException("Player is null");
        }
        Vector2 direction = (player.position - monster.transform.position).normalized;
        monster.transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private IEnumerator MoveRoutine()
    {
        WaitForSeconds waitDelay = new WaitForSeconds(waitTime);
        WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

        while (true)
        {
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
        string playerTag = GameConstants.Tags.Player.ToString();
        if (collision.gameObject.CompareTag(playerTag))
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
