using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;

    Vector2 inputVector;
    Rigidbody2D rigid;
    Animator animator;

    // 애니메이션 파라미터 이름
    const string IS_MOVING = "IsMoving";
    const string IS_MOVING_X = "isMovingX";
    const string IS_MOVING_Y = "isMovingY";
    const string MOVE_X = "moveX";
    const string MOVE_Y = "moveY";
    const string IS_MOUSE_CLICKED = "isMouseClicked";

    // 마지막 수평 방향 저장
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
        HandleInput(); // 입력 처리
        UpdateCharacterDirection(); // 캐릭터 방향 설정
        UpdateAnimationStates(); // 애니메이션 상태 업데이트
        HandleAttackInput(); // 공격 입력 처리
    }

    void HandleInput()
    {
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");

        // 대각선 이동 시 정규화
        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }
    }

    void UpdateCharacterDirection()
    {
        // 수평 입력이 있을 때만 마지막 방향 업데이트
        if (inputVector.x != 0)
        {
            lastHorizontalDirection = Mathf.Sign(inputVector.x);
        }

        // 스프라이트 방향 설정
        transform.localScale = new Vector3(lastHorizontalDirection, 1f, 1f);
    }

    void UpdateAnimationStates()
    {
        bool isMoving = inputVector.magnitude > 0;
        bool isMovingHorizontally = inputVector.x != 0;

        // 기본 이동 상태 설정
        animator.SetBool(IS_MOVING, isMoving);
        animator.SetFloat(MOVE_X, inputVector.x);
        animator.SetFloat(MOVE_Y, inputVector.y);

        // 수평 이동 우선순위 설정
        // 대각선 이동을 포함한 모든 수평 이동 시 horizontal 애니메이션 사용
        animator.SetBool(IS_MOVING_X, isMovingHorizontally);

        // 순수 수직 이동일 때만 수직 애니메이션 사용
        animator.SetBool(IS_MOVING_Y, inputVector.y != 0 && !isMovingHorizontally);
    }

    void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SwordAttack.Instance.Ani.SetTrigger(IS_MOUSE_CLICKED);
            // 여기에 공격 모션 추가
        }
    }

    void FixedUpdate()
    {
        Vector2 nextPosition = rigid.position + (inputVector * MoveSpeed * Time.fixedDeltaTime);
        rigid.MovePosition(nextPosition);
    }
}