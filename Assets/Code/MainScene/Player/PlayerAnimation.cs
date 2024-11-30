using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour
{
    public static PlayerAnimation Instance;
    public bool IsAttacking { get; private set; } = false;

    private float lastHorizontalDirection = 1f;
    private Animator animator;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("PlayerAnimation Instance�� �̹� �Ҵ�Ǿ����ϴ�.");
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimationStates();
        HandleAttackInput();
        UpdatePlayerDirection();
    }

    private void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && !IsAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        IsAttacking = true;
        animator.SetTrigger(AnimationParams.Player.IsMouseClicked);

        // �ִϸ��̼� ���� Ȯ�� �� �Ϸ� ���
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("AttackThePlayer"));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        IsAttacking = false;
    }

    private void UpdateAnimationStates()
    {
        Vector2 inputVector = PlayerMovement.Instance.InputVector;
        bool isMoving = inputVector.magnitude > 0;
        bool isMovingHorizontally = inputVector.x != 0;

        animator.SetBool(AnimationParams.Player.IsMoving, isMoving);
        animator.SetFloat(AnimationParams.Player.MoveX, inputVector.x);
        animator.SetFloat(AnimationParams.Player.MoveY, inputVector.y);
        animator.SetBool(AnimationParams.Player.IsMovingX, isMovingHorizontally);
        animator.SetBool(AnimationParams.Player.IsMovingY, inputVector.y != 0 && !isMovingHorizontally);
    }

    private void UpdatePlayerDirection()
    {
        float horizontalInput = PlayerMovement.Instance.InputVector.x;

        // ���� �Է��� �ִ� ��� ���� ������Ʈ
        if (horizontalInput != 0)
        {
            lastHorizontalDirection = Mathf.Sign(horizontalInput);
        }

        // �¿� ����
        transform.localScale = new Vector3(lastHorizontalDirection, 1f, 1f);
    }
}
