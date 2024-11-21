using UnityEngine;
using System.Collections;

public class BatMovement : MonoBehaviour
{
    [SerializeField] float speed = 2f;

    Transform player;
    Animator animator;
    MonsterHealth monsterHealth;
    Rigidbody2D rb;

    const string IS_ANGERING = "IsAngering";
    const string IS_DASHED = "IsDashed";

    bool isAttacking = false;
    bool isAngering = false;
    bool isDashing = false;
    bool isKnockBack = false;

    public float DetectionRange = 2f;
    float batSpeedIndex;
    float animatorSpeedIndex;

    void Awake()
    {
        animator = GetComponent<Animator>();
        monsterHealth = GetComponent<MonsterHealth>();
        rb = GetComponent<Rigidbody2D>();

        batSpeedIndex = speed;
        animatorSpeedIndex = animator.speed;
    }
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player 태그를 가진 객체를 찾을 수 없습니다.");
        }
    }
    void Update()
    {
        if (player == null) return;

        if (!isAttacking && !isKnockBack)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        if (Vector3.Distance(player.position, transform.position) < DetectionRange && !isAttacking && !isKnockBack)
        {
            StartCoroutine(AttackCoroutine());
        }
        UpdateCharacterDirection();
    }
    void UpdateCharacterDirection()
    {
        if (player == null) return;
        Vector2 direction = (player.position - transform.position).normalized;
        float scaleX = direction.x > 0 ? 1f : -1f;
        transform.localScale = new Vector3(scaleX, 1f, 1f);
    }

    IEnumerator AttackCoroutine()
    {
        if (isKnockBack)
        {
            yield return null;
        }
        if (isAttacking)
        {
            yield break;
        }
        isAttacking = true;
        isAngering = true;
        animator.SetBool(IS_ANGERING, isAngering);

        // 분노 애니메이션 재생
        float angerDuration = animator.runtimeAnimatorController.animationClips[1].length;
        animator.speed *= 1.3f;
        speed *= 1.5f;
        float time1 = 0;

        while (time1 < angerDuration * 3)
        {
            Vector2 dashDirection = (player.position - transform.position).normalized;
            rb.linearVelocity = dashDirection * (speed * 1.5f);
            time1 += Time.deltaTime;
            yield return null;
        }

        animator.speed = animatorSpeedIndex;
        speed = batSpeedIndex;

        // 돌진 상태로 전환
        isAngering = false;
        isDashing = true;
        animator.SetBool(IS_ANGERING, isAngering);
        animator.SetBool(IS_DASHED, isDashing);

        // 돌진 로직
        float dashSpeed = 10f;
        float dashDuration = 0.5f;

        float time2 = 0;
        while (time2 < dashDuration)
        {
            Vector2 dashDirection = (player.position - transform.position).normalized;
            rb.linearVelocity = dashDirection * dashSpeed;
            time2 += Time.deltaTime;
            yield return null;
        }

        // 대시 종료 후 속도 초기화
        rb.linearVelocity = Vector2.zero;

        // 모든 상태 초기화
        isDashing = false;
        isAttacking = false;
        animator.SetBool(IS_DASHED, isDashing);
        animator.SetBool(IS_ANGERING, false);
        yield return new WaitForSeconds(1);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && (isDashing || isAngering))
        {
            StopAllCoroutines(); // 모든 코루틴 중지

            // Rigidbody 속도 초기화
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // 넉백 처리
            isKnockBack = true;

            Vector2 knockbackDirection = (transform.position - player.position).normalized;
            rb.AddForce(knockbackDirection * 25f, ForceMode2D.Impulse);

            // 상태 및 애니메이션 초기화
            ResetStates();
            StartCoroutine(ResetKnockback()); // 넉백 해제
        }
    }

    // 상태 초기화 함수
    void ResetStates()
    {
        isAttacking = false;
        isDashing = false;
        isAngering = false;

        // 애니메이터 상태 초기화
        if (animator.GetBool(IS_ANGERING))
        {
            animator.SetBool(IS_ANGERING, false); // 분노 상태 해제
        }

        if (animator.GetBool(IS_DASHED))
        {
            animator.SetBool(IS_DASHED, false); // 돌진 상태 해제
        }

        // 속도 및 애니메이터 속도 복원
        speed = batSpeedIndex;
        animator.speed = animatorSpeedIndex;
    }

    // 넉백 상태를 해제하는 코루틴
    IEnumerator ResetKnockback()
    {
        yield return new WaitForSeconds(0.5f);
        isKnockBack = false;
    }

}