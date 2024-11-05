using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;

    Vector2 inputVector;
    Rigidbody2D rigid;
    Animator animator;

    // �ִϸ��̼� �Ķ���� �̸�
    const string IS_MOVING = "IsMoving";
    const string MOVE_Y = "moveY";
    const string IS_MOVING_X = "isMovingX";
    const string IS_MOVING_Y = "isMovingY";

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

        UpdateAnimationStates();
    }

    void UpdateAnimationStates()
    {
        // ĳ���Ͱ� �̵� ������ Ȯ��
        if (inputVector.magnitude > 0)
        {
            animator.SetBool(IS_MOVING, true);
            animator.SetFloat(MOVE_Y, inputVector.y);
        }
        else
        {
            animator.SetBool(IS_MOVING, false);
        }
        // X �� Y �̵� ���� ������Ʈ
        animator.SetBool(IS_MOVING_X, inputVector.x != 0);
        animator.SetBool(IS_MOVING_Y, inputVector.y != 0);
    }

    void FixedUpdate()
    {
        Vector2 nextVector = inputVector * MoveSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVector);
    }
}