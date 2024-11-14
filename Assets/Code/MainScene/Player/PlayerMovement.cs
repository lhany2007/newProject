using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    public Rigidbody2D Rigid;
    public float MoveSpeed = 5f;
    public float knockbackForce = 5f; // ³Ë¹é Èû
    public float knockbackDuration = 0.5f; // ³Ë¹é Áö¼Ó ½Ã°£

    private Vector2 inputVector;
    private Animator animator;
    private bool isKnockedBack = false;
    private float knockbackEndTime;

    const string IS_MOVING = "IsMoving";
    const string IS_MOVING_X = "isMovingX";
    const string IS_MOVING_Y = "isMovingY";
    const string MOVE_X = "moveX";
    const string MOVE_Y = "moveY";
    const string IS_MOUSE_CLICKED = "isMouseClicked";

    private float lastHorizontalDirection = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        Rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isKnockedBack)
        {
            HandleInput();
        }

        UpdateCharacterDirection();
        UpdateAnimationStates();
        HandleAttackInput();
    }

    void HandleInput()
    {
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");
        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }
    }

    void UpdateCharacterDirection()
    {
        if (inputVector.x != 0)
        {
            lastHorizontalDirection = Mathf.Sign(inputVector.x);
        }
        transform.localScale = new Vector3(lastHorizontalDirection, 1f, 1f);
    }

    void UpdateAnimationStates()
    {
        bool isMoving = inputVector.magnitude > 0;
        bool isMovingHorizontally = inputVector.x != 0;

        animator.SetBool(IS_MOVING, isMoving);
        animator.SetFloat(MOVE_X, inputVector.x);
        animator.SetFloat(MOVE_Y, inputVector.y);

        animator.SetBool(IS_MOVING_X, isMovingHorizontally);
        animator.SetBool(IS_MOVING_Y, inputVector.y != 0 && !isMovingHorizontally);
    }

    void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SwordAttack.Instance.Ani.SetTrigger(IS_MOUSE_CLICKED);
        }
    }

    public void ApplyKnockback(Vector3 direction)
    {
        isKnockedBack = true;
        knockbackEndTime = Time.time + knockbackDuration;

        Rigid.linearVelocity = Vector2.zero;
        Rigid.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        if (isKnockedBack)
        {
            if (Time.time >= knockbackEndTime)
            {
                isKnockedBack = false;
            }
        }
        else
        {
            Vector2 nextPosition = Rigid.position + (inputVector * MoveSpeed * Time.fixedDeltaTime);
            Rigid.MovePosition(nextPosition);
        }
    }
}
