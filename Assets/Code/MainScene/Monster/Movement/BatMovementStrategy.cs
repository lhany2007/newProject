using UnityEngine;
using System.Collections;
using System;

public class BatMovementStrategy : IMonsterMovementStrategy
{
    private GameObject monster;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private MonsterStats.Stats stats;

    [Header("Movement Settings")]
    private float detectionRange = 2f;
    private float dashSpeed = 10f;
    private float dashDuration = 0.5f;
    private float knockbackForce = 25f;
    private float knockbackDuration = 0.5f;
    private float angerSpeedMultiplier = 1.5f;
    private float angerAnimationSpeedMultiplier = 1.3f;

    private MonsterMovementState currentState;
    private float baseSpeed;
    private float baseAnimatorSpeed;

    public void Initialize(GameObject monster, MonsterStats.Stats monsterStats)
    {
        this.monster = monster;
        rb = monster.GetComponent<Rigidbody2D>();
        animator = monster.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(GameConstants.Tags.Player.ToString())?.transform;

        stats = monsterStats;
        baseSpeed = stats.Speed;
        baseAnimatorSpeed = animator.speed;
        currentState = MonsterMovementState.Idle;

        monster.StartCoroutine(BehaviorRoutine());
    }

    public void Move()
    {
        if (player == null)
        {
            throw new NullReferenceException("Player is null");
        }

        // Idle ������ ��쿡��
        if (currentState == MonsterMovementState.Idle)
        {
            MoveTowardsPlayer();
        }
        UpdateDirection();
    }

    // �÷��̾� ������ �̵��ϴ� �Լ�
    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - monster.transform.position).normalized;
        monster.transform.position += (Vector3)direction * baseSpeed * Time.deltaTime;
    }

    // ������ �̵� ������ ������Ʈ�ϴ� �Լ�
    private void UpdateDirection()
    {
        Vector3 direction = (player.position - monster.transform.position).normalized;
        monster.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1); // ���⿡ ���� ���� ������ ���� (�¿� ����)
    }

    // ������ �ൿ ��ƾ
    private IEnumerator BehaviorRoutine()
    {
        WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

        while (true)
        {
            float distanceToPlayer = Vector2.Distance(player.position, monster.transform.position); // �÷��̾���� �Ÿ� ���

            // �÷��̾ ���� ���� �ȿ� ���� ��� && Idle ������ ���
            if (distanceToPlayer < detectionRange && currentState == MonsterMovementState.Idle)
            {
                yield return monster.StartCoroutine(AttackSequence()); // ����
            }

            yield return endOfFrame;
        }
    }

    private IEnumerator AttackSequence()
    {
        currentState = MonsterMovementState.Anger;
        animator.SetBool(GameConstants.AnimationParams.IsAngering.ToString(), true);

        // �г� ����
        SetAngerState();
        yield return monster.StartCoroutine(AngerChase());
        ResetAngerState();

        // ���� ����
        currentState = MonsterMovementState.Dash;
        animator.SetBool(GameConstants.AnimationParams.IsAngering.ToString(), false);
        animator.SetBool(GameConstants.AnimationParams.IsDashed.ToString(), true);

        yield return monster.StartCoroutine(PerformDash());

        // �ʱ� ���·� ����
        yield return monster.StartCoroutine(ResetToIdle());
    }

    // �г� ���·� ����
    private void SetAngerState()
    {
        animator.speed *= angerAnimationSpeedMultiplier; // �ִϸ��̼� �ӵ� ����
        baseSpeed *= angerSpeedMultiplier; // �̵� �ӵ� ����
    }

    // �г� ���� ����
    private void ResetAngerState()
    {
        animator.speed = baseAnimatorSpeed; // �ִϸ��̼� �ӵ� �ʱ�ȭ
        baseSpeed = stats.Speed; // �̵� �ӵ� �ʱ�ȭ
    }

    // �г� ���¿��� �÷��̾ �����ϴ� �Լ�
    private IEnumerator AngerChase()
    {
        float angerDuration = animator.runtimeAnimatorController.animationClips[1].length * 3;
        float elapsed = 0;

        while (elapsed < angerDuration)
        {
            Vector2 direction = (player.position - monster.transform.position).normalized;
            rb.linearVelocity = direction * (baseSpeed * angerSpeedMultiplier);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // �÷��̾�� ����
    private IEnumerator PerformDash()
    {
        float elapsed = 0;

        while (elapsed < dashDuration)
        {
            Vector2 direction = (player.position - monster.transform.position).normalized;
            rb.linearVelocity = direction * dashSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
    }

    // Idle ���·� �����ϴ� �Լ�(���Ŀ� ���� �ִϸ��̼� ���� ��ũ��Ʈ ����Ŵ�)
    private IEnumerator ResetToIdle()
    {
        currentState = MonsterMovementState.Idle;
        animator.SetBool(GameConstants.AnimationParams.IsDashed.ToString(), false);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        string playerTag = GameConstants.Tags.Player.ToString();
        if (collision.gameObject.CompareTag(playerTag))
        {
            AttackThePlayer();
        }
        // �г� ���°ų�, ���� ���¿��� �ε�ġ��
        if (currentState == MonsterMovementState.Anger && currentState == MonsterMovementState.Dash)
        {
            HandleCollisionWithPlayer();
        }
    }

    private void AttackThePlayer()
    {
        float damage = stats.Damage;
        Vector3 myPos = monster.transform.position;
        bool isKnockback = true;
        MonsterManager.Instance.AttackPlayerOnCollision(damage, myPos, isKnockback);
    }

    private void HandleCollisionWithPlayer()
    {
        MonoBehaviour component = monster.GetComponent<MonoBehaviour>();

        component.StopAllCoroutines();  // ������ ���� ���� ��� �ڷ�ƾ�� ����
        currentState = MonsterMovementState.Knockback; // �˹� ���� ���·� ��ȯ

        ResetAngerState();
        animator.SetBool(GameConstants.AnimationParams.IsAngering.ToString(), false);
        animator.SetBool(GameConstants.AnimationParams.IsDashed.ToString(), false);
        
        Knockback();
        
        monster.StartCoroutine(KnockbackRecovery());
    }

    private void Knockback()
    {
        rb.linearVelocity = Vector2.zero; // �ӵ� �ʱ�ȭ
        KnockbackManager.Instance.ApplyKnockback(rb, knockbackForce, monster.transform.position, player.position);
    }

    // �˹� ȸ�� �Լ�
    private IEnumerator KnockbackRecovery()
    {
        yield return new WaitForSeconds(knockbackDuration);
        currentState = MonsterMovementState.Idle;
        monster.StartCoroutine(BehaviorRoutine());
    }

    public void Stop()
    {
        currentState = MonsterMovementState.Idle;
        rb.linearVelocity = Vector2.zero;
    }
}