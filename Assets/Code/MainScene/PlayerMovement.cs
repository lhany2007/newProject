using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;

    Vector2 inputVector;
    Rigidbody2D rigid;
    Animator animator;

    // �ִϸ��̼� �Ķ���� �̸�
    const string IS_MOVING = "IsMoving";
    const string MOVE_X = "moveX";
    const string MOVE_Y = "moveY";
    const string IS_MOVING_X = "isMovingX";
    const string IS_MOVING_Y = "isMovingY";
    const string IS_MOUSE_CLICKED = "isMouseClicked";

    // ������ ������ ������ ���� �߰�
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

        // �밢�� �̵� �� ����ȭ
        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }

        // ���� �Է��� ���� ���� ������ ���� ������Ʈ
        if (inputVector.x != 0)
        {
            lastHorizontalDirection = Mathf.Sign(inputVector.x);
        }

        // ������ ������ �������� �¿� ����
        transform.localScale = new Vector3(lastHorizontalDirection, 1, 1);

        if (Input.GetMouseButtonDown(0))
        {
            SwordAttackAnimation.Instance.Ani.SetTrigger(IS_MOUSE_CLICKED);
            /* 
            ���⿡ ���� �ִϸ��̼� �߰�
            animator.SetBool(IS_MOUSE_CLICKED);
            */
        }

        UpdateAnimationStates();
    }

    void UpdateAnimationStates()
    {
        // �⺻ �̵� �÷���
        animator.SetBool(IS_MOVING, inputVector.magnitude > 0);
        animator.SetFloat(MOVE_X, inputVector.x);
        animator.SetFloat(MOVE_Y, inputVector.y);

        // �밢�� �̵� �� ���� �ִϸ��̼��� �켱���� ����
        bool isDiagonal = inputVector.x != 0 && inputVector.y != 0;
        bool isHorizontalPriority = Mathf.Abs(inputVector.x) >= Mathf.Abs(inputVector.y);

        // ����/���� �÷��� ����
        animator.SetBool(IS_MOVING_X, inputVector.x != 0 && (!isDiagonal || isHorizontalPriority));
        animator.SetBool(IS_MOVING_Y, inputVector.y != 0 && (!isDiagonal || !isHorizontalPriority));
    }


    void FixedUpdate()
    {
        Vector2 nextVector = inputVector * MoveSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVector);
    }
}