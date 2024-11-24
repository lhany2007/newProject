using UnityEditorInternal;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    Rigidbody2D rb;
    Animator animator;

    [SerializeField] float speed = 5f;
    [SerializeField] float knockbackDuration = 0.5f; // �˹� ���� �ð�
    
    public Vector2 InputVector;

    bool isKnockedBack = false;
    // bool isInputLocked = false; // �Է� ��� ����
    float knockbackEndTime;

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
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        /*
        if (!isKnockBack && !isInputLocked) // �Է� ��� ���� üũ
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

    public void ApplyKnockback(Vector3 pos, Vector3 targetVec)
    {
        isKnockedBack = true;
        knockbackEndTime = Time.time + knockbackDuration;
        animator.SetBool(PlayerAnimation.Instance.IS_KNOCKEDBACK, isKnockedBack);
        GameManager.Instance.Knockback(rb, 5f, pos, targetVec, animator);
    }

    void FixedUpdate()
    {
        if (isKnockedBack)
        {
            if (Time.time >= knockbackEndTime)
            {
                isKnockedBack = false;
                animator.SetBool(PlayerAnimation.Instance.IS_KNOCKEDBACK, isKnockedBack);
            }
        }
        else
        {
            Vector2 nextPosition = rb.position + (InputVector * speed * Time.fixedDeltaTime);
            rb.MovePosition(nextPosition);
        }
    }

    /*
    // �Է� ���/����
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