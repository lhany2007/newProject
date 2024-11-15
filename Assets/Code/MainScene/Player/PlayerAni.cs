using UnityEngine;
using System.Collections;

public class PlayerAni : MonoBehaviour
{
    public static PlayerAni Instance;

    const string IS_MOVING = "IsMoving";
    const string IS_MOVING_X = "isMovingX";
    const string IS_MOVING_Y = "isMovingY";
    const string MOVE_X = "moveX";
    const string MOVE_Y = "moveY";
    const string IS_MOUSE_CLICKED = "isMouseClicked";

    public bool IsAttacking = false;

    Animator animator;
    PlayerMovement player;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        player = PlayerMovement.Instance;
    }

    void Update()
    {
        UpdateAnimationStates();
        HandleAttackInput();
        UpdateCharacterDirection();
    }

    void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && !IsAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator AttackRoutine()
    {
        IsAttacking = true;
        animator.SetTrigger(IS_MOUSE_CLICKED);

        // 현재 애니메이션 상태 정보 가져오기
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션이 실제로 시작될 때까지 대기
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            yield return null;
        }

        // 애니메이션 완료까지 대기
        float normalizedTime = 0;
        while (normalizedTime < 1.0f)
        {
            normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            yield return null;
        }

        // 상태 초기화
        IsAttacking = false;
    }

    void UpdateAnimationStates()
    {
        bool isMoving = player.InputVector.magnitude > 0;
        bool isMovingHorizontally = player.InputVector.x != 0;

        animator.SetBool(IS_MOVING, isMoving);
        animator.SetFloat(MOVE_X, player.InputVector.x);
        animator.SetFloat(MOVE_Y, player.InputVector.y);

        animator.SetBool(IS_MOVING_X, isMovingHorizontally);
        animator.SetBool(IS_MOVING_Y, player.InputVector.y != 0 && !isMovingHorizontally);
    }

    /// <summary>
    /// 좌우 반전
    /// </summary>
    void UpdateCharacterDirection()
    {
        if (player.InputVector.x != 0)
        {
            player.lastHorizontalDirection = Mathf.Sign(player.InputVector.x);
        }
        transform.localScale = new Vector3(player.lastHorizontalDirection, 1f, 1f);
    }
}
