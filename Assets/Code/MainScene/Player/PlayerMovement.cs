using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    Rigidbody2D rb;
    Animator animator;
    [SerializeField] float speed = 5f;
    [SerializeField] float knockbackDuration = 0.5f; // ³Ë¹é Áö¼Ó ½Ã°£
    [SerializeField] private float knockbackForce = 5f; // ³Ë¹é
    public Vector2 InputVector;
    bool isKnockedBack = false;
    float knockbackEndTime;
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Player Instance°¡ ÀÌ¹Ì ÇÒ´çµÊ");
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
        PlayerInput();
    }
    void PlayerInput()
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
        if (isKnockedBack) return;
        isKnockedBack = true;
        knockbackEndTime = Time.time + knockbackDuration;
        animator.SetBool(AnimationParams.Player.IsKnockedBack, isKnockedBack);
        AnimationManager.Instance.StopAnimation(animator);
        KnockbackManager.Instance.ApplyKnockback(rb, knockbackForce, pos, targetVec);
        AnimationManager.Instance.StartAnimation(animator);
    }
    void FixedUpdate()
    {
        if (isKnockedBack)
        {
            if (Time.time >= knockbackEndTime)
            {
                isKnockedBack = false;
                animator.SetBool(AnimationParams.Player.IsKnockedBack, isKnockedBack);
            }
        }
        else
        {
            Vector2 nextPosition = rb.position + (InputVector * speed * Time.fixedDeltaTime);
            rb.MovePosition(nextPosition);
        }
    }
}
