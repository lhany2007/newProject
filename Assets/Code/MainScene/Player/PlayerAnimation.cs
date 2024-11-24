using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour
{
    public static PlayerAnimation Instance;

    private const string IS_MOVING = "IsMoving";
    private const string IS_MOVING_X = "isMovingX";
    private const string IS_MOVING_Y = "isMovingY";
    private const string MOVE_X = "moveX";
    private const string MOVE_Y = "moveY";
    private const string IS_MOUSE_CLICKED = "isMouseClicked";
    public readonly string IS_KNOCKEDBACK = "isKnockedBack";

    public bool IsAttacking = false;


    private float lastHorizontalDirection = 1f;

    Animator animator;
    PlayerMovement player;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Player Instance�� �̹� �Ҵ��");
        }
        Instance = this;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerMovement>(); // ||  player = PlayerMovement.Instance;
    }

    void Update()
    {
        UpdateAnimationStates();
        HandleAttackInput();
        UpdatePlayerDirection();
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

        // ���� �ִϸ��̼� ���� ���� ��������
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // �ִϸ��̼��� ���۵� ������ ���
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            yield return null;
        }

        // �ִϸ��̼� �Ϸ���� ���
        float normalizedTime = 0;
        while (normalizedTime < 1.0f)
        {
            normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            yield return null;
        }

        // ���� �ʱ�ȭ
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
    /// �¿� ����
    /// </summary>
    void UpdatePlayerDirection()
    {
        // ���� �Է��� ���� ���� ������ ���� ������Ʈ
        if (PlayerMovement.Instance.InputVector.x != 0)
        {
            lastHorizontalDirection = Mathf.Sign(PlayerMovement.Instance.InputVector.x);
        }

        // ��������Ʈ ���� ����
        transform.localScale = new Vector3(lastHorizontalDirection, 1f, 1f);
    }
}
