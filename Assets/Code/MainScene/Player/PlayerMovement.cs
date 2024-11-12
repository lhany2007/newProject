using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;

    Vector2 inputVector;
    Rigidbody2D rigid;
    Animator animator;

    // �ִϸ��̼� �Ķ���� �̸�
    const string IS_MOVING = "IsMoving";
    const string IS_MOVING_X = "isMovingX";
    const string IS_MOVING_Y = "isMovingY";
    const string MOVE_X = "moveX";
    const string MOVE_Y = "moveY";
    const string IS_MOUSE_CLICKED = "isMouseClicked";

    // ������ ���� ���� ����
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
        HandleInput(); // �Է� ó��
        UpdateCharacterDirection(); // ĳ���� ���� ����
        UpdateAnimationStates(); // �ִϸ��̼� ���� ������Ʈ
        HandleAttackInput(); // ���� �Է� ó��
    }

    void HandleInput()
    {
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");

        // �밢�� �̵� �� ����ȭ
        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }
    }

    void UpdateCharacterDirection()
    {
        // ���� �Է��� ���� ���� ������ ���� ������Ʈ
        if (inputVector.x != 0)
        {
            lastHorizontalDirection = Mathf.Sign(inputVector.x);
        }

        // ��������Ʈ ���� ����
        transform.localScale = new Vector3(lastHorizontalDirection, 1f, 1f);
    }

    void UpdateAnimationStates()
    {
        bool isMoving = inputVector.magnitude > 0;
        bool isMovingHorizontally = inputVector.x != 0;

        // �⺻ �̵� ���� ����
        animator.SetBool(IS_MOVING, isMoving);
        animator.SetFloat(MOVE_X, inputVector.x);
        animator.SetFloat(MOVE_Y, inputVector.y);

        // ���� �̵� �켱���� ����
        // �밢�� �̵��� ������ ��� ���� �̵� �� horizontal �ִϸ��̼� ���
        animator.SetBool(IS_MOVING_X, isMovingHorizontally);

        // ���� ���� �̵��� ���� ���� �ִϸ��̼� ���
        animator.SetBool(IS_MOVING_Y, inputVector.y != 0 && !isMovingHorizontally);
    }

    void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SwordAttack.Instance.Ani.SetTrigger(IS_MOUSE_CLICKED);
            // ���⿡ ���� ��� �߰�
        }
    }

    void FixedUpdate()
    {
        Vector2 nextPosition = rigid.position + (inputVector * MoveSpeed * Time.fixedDeltaTime);
        rigid.MovePosition(nextPosition);
    }
}