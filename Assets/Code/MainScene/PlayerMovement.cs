using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;

    Vector2 inputVector;
    Rigidbody2D rigid;
    Animator animator;

    // 애니메이션 파라미터 이름
    const string IS_MOVING = "IsMoving";
    const string MOVE_X = "moveX";
    const string MOVE_Y = "moveY";
    const string IS_MOVING_X = "isMovingX";
    const string IS_MOVING_Y = "isMovingY";
    const string IS_MOUSE_CLICKED = "isMouseClicked";

    // 마지막 방향을 저장할 변수 추가
    float lastHorizontalDirection = 1f;

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

        // 수평 입력이 있을 때만 마지막 방향 업데이트
        if (inputVector.x != 0)
        {
            lastHorizontalDirection = Mathf.Sign(inputVector.x);
        }

        // 마지막 방향을 기준으로 좌우 반전
        transform.localScale = new Vector3(lastHorizontalDirection, 1, 1);

        if (Input.GetMouseButtonDown(0))
        {
            SwordAttackAnimation.Instance.Ani.SetTrigger(IS_MOUSE_CLICKED);
            /* 
            여기에 공격 애니매이션 추가
            animator.SetBool(IS_MOUSE_CLICKED);
            */
        }

        UpdateAnimationStates();
    }

    void UpdateAnimationStates()
    {
        // 기본 이동 플래그
        animator.SetBool(IS_MOVING, inputVector.magnitude > 0);
        animator.SetFloat(MOVE_X, inputVector.x);
        animator.SetFloat(MOVE_Y, inputVector.y);

        // 대각선 이동 시 수평 애니메이션을 우선으로 설정
        bool isDiagonal = inputVector.x != 0 && inputVector.y != 0;
        bool isHorizontalPriority = Mathf.Abs(inputVector.x) >= Mathf.Abs(inputVector.y);

        // 수평/수직 플래그 설정
        animator.SetBool(IS_MOVING_X, inputVector.x != 0 && (!isDiagonal || isHorizontalPriority));
        animator.SetBool(IS_MOVING_Y, inputVector.y != 0 && (!isDiagonal || !isHorizontalPriority));
    }


    void FixedUpdate()
    {
        Vector2 nextVector = inputVector * MoveSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVector);
    }
}