using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    public Rigidbody2D Rigid;
    public float MoveSpeed = 5f;
    public float knockbackForce = 5f; // �˹� ��
    public float knockbackDuration = 0.5f; // �˹� ���� �ð�
    public float lastHorizontalDirection = 1f;
    
    public Vector2 InputVector;

    bool isKnockedBack = false;
    // bool isInputLocked = false; // �Է� ��� ���¸� �����ϴ� ����
    float knockbackEndTime;

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
            Vector2 nextPosition = Rigid.position + (InputVector * MoveSpeed * Time.fixedDeltaTime);
            Rigid.MovePosition(nextPosition);
        }
    }

    /*
    // �Է� ���/����
    public void LockInput()
    {
        isInputLocked = true;
        InputVector = Vector2.zero;
        Rigid.linearVelocity = Vector2.zero;
    }

    public void UnlockInput()
    {
        isInputLocked = false;
    }
    */
}