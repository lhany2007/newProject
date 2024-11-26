using UnityEngine;

public class KnockbackManager : MonoBehaviour
{
    public static KnockbackManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ApplyKnockback(Rigidbody2D rb, float knockbackForce, Vector3 pos, Vector3 targetVec)
    {
        rb.AddForce((pos - targetVec).normalized * knockbackForce, ForceMode2D.Impulse);
    }
}