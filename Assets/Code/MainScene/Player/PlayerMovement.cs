using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    Rigidbody2D rb;

    [SerializeField] float speed = 5f;
    [SerializeField] float knockbackForce = 5f; // 넉백 힘
    [SerializeField] float knockbackDuration = 0.5f; // 넉백 지속 시간
    public float LastHorizontalDirection = 1f;
    
    public Vector2 InputVector;

    bool isKnockedBack = false;
    // bool isInputLocked = false; // 입력 잠금 상태
    float knockbackEndTime;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Player Instance가 이미 할당됨");
        }
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        /*
        if (!isKnockBack && !isInputLocked) // 입력 잠금 상태 체크
        {
            HandleInput();
        }
        */
        HandleInput();
    }

    void HandleInput()
    {
        InputVector.x = Input.GetAxisRaw("Horizontal");
        InputVector.y = Input.GetAxisRaw("Vertical");

        if (InputVector.magnitude > 1)
        {
            InputVector.Normalize();
        }
    }

    public void ApplyKnockback(Vector3 direction)
    {
        isKnockedBack = true;
        knockbackEndTime = Time.time + knockbackDuration;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
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
            Vector2 nextPosition = rb.position + (InputVector * speed * Time.fixedDeltaTime);
            rb.MovePosition(nextPosition);
        }
    }

    /*
    // 입력 잠금/해제
    public void LockInput()
    {
        isInputLocked = true;
        InputVector = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    public void UnlockInput()
    {
        isInputLocked = false;
    }
    */
}