using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;

    private Vector2 inputVector;
    private Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        MoveSpeed = 5f;
    }

    void Update()
    {
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");

        // 대각선 움직임 정규화
        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }
    }

    void FixedUpdate()
    {
        Vector2 nextVector = inputVector * MoveSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVector);
    }
}