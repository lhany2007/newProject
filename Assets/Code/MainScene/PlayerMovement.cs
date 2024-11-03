using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;
    Vector2 inputVector;
    Rigidbody2D rigid;
    Animator animator;

    const string IS_MOVING = "IsMoving";
    const string MOVE_Y = "moveY";
    const string IS_MOVING_X = "isMovingX";
    const string IS_MOVING_Y = "isMovingY";
    const string IS_UNDER_DIAGONAI_UP = "isUnderDiagonalUp";
    const string IS_UNDER_DIAGONAI_DOWN = "isUnderDiagonalDown";
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        MoveSpeed = 5f;
    }

    void Update()
    {
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");
        // 대각선 움직임 정규화
        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }
        // 좌우반전
        if (inputVector.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(inputVector.x), 1, 1);
        }
    }
    void UpdateAnimationStates()
    {
        if (inputVector.magnitude > 0)
        {
            animator.SetBool(IS_MOVING, true);

            // 대각선 움직임 확인 (x와 y가 모두 0이 아닌 경우)
            if (Mathf.Abs(inputVector.x) > 0.1f && Mathf.Abs(inputVector.y) > 0.1f)
            {
                if (inputVector.y > 0)
                {
                    animator.SetBool(IS_UNDER_DIAGONAI_UP, true);
                    animator.SetBool(IS_UNDER_DIAGONAI_DOWN, false);
                }
                else
                {
                    animator.SetBool(IS_UNDER_DIAGONAI_DOWN, true);
                    animator.SetBool(IS_UNDER_DIAGONAI_UP, false);
                }
            }
            else
            {
                // 대각선이 아닌 경우 두 상태 모두 false
                animator.SetBool(IS_UNDER_DIAGONAI_UP, false);
                animator.SetBool(IS_UNDER_DIAGONAI_DOWN, false);
            }

            animator.SetFloat(MOVE_Y, inputVector.y);
        }
        else
        {
            // 움직임이 없는 경우 모든 상태 리셋
            animator.SetBool(IS_MOVING, false);
            animator.SetBool(IS_UNDER_DIAGONAI_UP, false);
            animator.SetBool(IS_UNDER_DIAGONAI_DOWN, false);
        }

        // X축 움직임 상태 업데이트
        animator.SetBool(IS_MOVING_X, inputVector.x != 0);

        // Y축 움직임 상태 업데이트
        animator.SetBool(IS_MOVING_Y, inputVector.y != 0);
    }

    void FixedUpdate()
    {
        Vector2 nextVector = inputVector * MoveSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVector);
        UpdateAnimationStates();
    }
}