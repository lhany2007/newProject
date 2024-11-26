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

        // Idle 상태일 경우에만
        if (currentState == MonsterMovementState.Idle)
        {
            MoveTowardsPlayer();
        }
        UpdateDirection();
    }

    // 플레이어 쪽으로 이동하는 함수
    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - monster.transform.position).normalized;
        monster.transform.position += (Vector3)direction * baseSpeed * Time.deltaTime;
    }

    // 몬스터의 이동 방향을 업데이트하는 함수
    private void UpdateDirection()
    {
        Vector3 direction = (player.position - monster.transform.position).normalized;
        monster.transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1); // 방향에 따라 몬스터 스케일 변경 (좌우 반전)
    }

    // 몬스터의 행동 루틴
    private IEnumerator BehaviorRoutine()
    {
        WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

        while (true)
        {
            float distanceToPlayer = Vector2.Distance(player.position, monster.transform.position); // 플레이어와의 거리 계산

            // 플레이어가 감지 범위 안에 있을 경우 && Idle 상태일 경우
            if (distanceToPlayer < detectionRange && currentState == MonsterMovementState.Idle)
            {
                yield return monster.StartCoroutine(AttackSequence()); // 공격
            }

            yield return endOfFrame;
        }
    }

    private IEnumerator AttackSequence()
    {
        currentState = MonsterMovementState.Anger;
        animator.SetBool(GameConstants.AnimationParams.IsAngering.ToString(), true);

        // 분노 상태
        SetAngerState();
        yield return monster.StartCoroutine(AngerChase());
        ResetAngerState();

        // 돌진 상태
        currentState = MonsterMovementState.Dash;
        animator.SetBool(GameConstants.AnimationParams.IsAngering.ToString(), false);
        animator.SetBool(GameConstants.AnimationParams.IsDashed.ToString(), true);

        yield return monster.StartCoroutine(PerformDash());

        // 초기 상태로 리셋
        yield return monster.StartCoroutine(ResetToIdle());
    }

    // 분노 상태로 설정
    private void SetAngerState()
    {
        animator.speed *= angerAnimationSpeedMultiplier; // 애니메이션 속도 증가
        baseSpeed *= angerSpeedMultiplier; // 이동 속도 증가
    }

    // 분노 상태 리셋
    private void ResetAngerState()
    {
        animator.speed = baseAnimatorSpeed; // 애니메이션 속도 초기화
        baseSpeed = stats.Speed; // 이동 속도 초기화
    }

    // 분노 상태에서 플레이어를 추적하는 함수
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

    // 플레이어에게 돌진
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

    // Idle 상태로 리셋하는 함수(추후에 몬스터 애니메이션 관리 스크립트 만들거다)
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
        // 분노 상태거나, 돌진 상태에서 부딪치면
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

        component.StopAllCoroutines();  // 기존에 실행 중인 모든 코루틴을 멈춤
        currentState = MonsterMovementState.Knockback; // 넉백 중인 상태로 전환

        ResetAngerState();
        animator.SetBool(GameConstants.AnimationParams.IsAngering.ToString(), false);
        animator.SetBool(GameConstants.AnimationParams.IsDashed.ToString(), false);
        
        Knockback();
        
        monster.StartCoroutine(KnockbackRecovery());
    }

    private void Knockback()
    {
        rb.linearVelocity = Vector2.zero; // 속도 초기화
        KnockbackManager.Instance.ApplyKnockback(rb, knockbackForce, monster.transform.position, player.position);
    }

    // 넉백 회복 함수
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