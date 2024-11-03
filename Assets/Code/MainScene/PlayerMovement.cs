using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;

    Vector2 inputVector;
    Rigidbody2D rigid;
    Animator animator;

    // 애니메이션 파라미터 이름
    const string IS_MOVING = "IsMoving";
    const string MOVE_Y = "moveY";
    const string IS_MOVING_X = "isMovingX";
    const string IS_MOVING_Y = "isMovingY";

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

        // 대각선 이동 시 정규화
        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }

        // 좌우 반전
        if (inputVector.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(inputVector.x), 1, 1);
        }

        UpdateAnimationStates();
    }

    void UpdateAnimationStates()
    {
        // 캐릭터가 이동 중인지 확인
        if (inputVector.magnitude > 0)
        {
            animator.SetBool(IS_MOVING, true);
            animator.SetFloat(MOVE_Y, inputVector.y);
        }
        else
        {
            animator.SetBool(IS_MOVING, false);
        }

        // X 및 Y 이동 상태 업데이트
        animator.SetBool(IS_MOVING_X, inputVector.x != 0);
        animator.SetBool(IS_MOVING_Y, inputVector.y != 0);
    }

    void FixedUpdate()
    {
        Vector2 nextVector = inputVector * MoveSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVector);
    }
}
