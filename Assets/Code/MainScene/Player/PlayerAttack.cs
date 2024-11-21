using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack Instance;

    const string MONSTER_LAYER = "Monster";

    bool isAttacking = false;

    PolygonCollider2D SwordCollider;
    Animator animator;
    Dictionary<int, Vector2[]> colliderShapes;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        animator = PlayerAni.Instance.GetComponent<Animator>();
        SwordCollider = GetComponent<PolygonCollider2D>();

        colliderShapes = new Dictionary<int, Vector2[]>(); // 여기서 초기화

        InitializeColliderShapes();
        UpdateColliderShape(0); // 기본 콜라이더 모양 설정
    }


    void Update()
    {
        // 애니메이터의 현재 상태를 확인
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            isAttacking = true;
            int frameIndex = GetCurrentFrameIndex();
            UpdateColliderShape(frameIndex);
        }
        else
        {
            if (isAttacking)
            {
                isAttacking = false;
                UpdateColliderShape(0); // Default 상태
            }
        }
    }

    // 현재 애니메이션 프레임 인덱스를 반환하는 함수
    int GetCurrentFrameIndex()
    {
        float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        int frameIndex = Mathf.Clamp(Mathf.FloorToInt(normalizedTime * 6), 0, 5);
        return frameIndex;
    }

    // 콜라이더 모양 업데이트 함수
    void UpdateColliderShape(int shapeIndex)
    {
        if (colliderShapes.ContainsKey(shapeIndex))
        {
            SwordCollider.SetPath(0, colliderShapes[shapeIndex]);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(MONSTER_LAYER))
        {
            MonsterHealth monsterHealth = other.GetComponent<MonsterHealth>();
            if (monsterHealth != null)
            {
                monsterHealth.TakeDamage(WeaponManager.Instance.WeaponDamageDictionary["Sword"]);
            }
        }
    }

    void InitializeColliderShapes()
    {
        colliderShapes[0] = new Vector2[]
        {
            new Vector2(-0.6131981f, -0.1019547f),
            new Vector2(-0.6170515f, -0.1001165f),
            new Vector2(-0.6246028f, -0.09046727f)
        };
        colliderShapes[1] = new Vector2[]
        {
            new Vector2(-0.6131981f, -0.1019547f),
            new Vector2(-0.6170515f, -0.1001165f),
            new Vector2(-0.6246028f, -0.09046727f)
        };
        colliderShapes[2] = new Vector2[]
        {
            new Vector2(-0.6131981f, -0.1019547f),
            new Vector2(-0.6170515f, -0.1001165f),
            new Vector2(-0.6246028f, -0.09046727f)
        };
        colliderShapes[3] = new Vector2[]
        {
            new Vector2(-0.1455676f, 0.4725608f),
            new Vector2(-0.2407014f, 0.07932141f),
            new Vector2(0.1497979f, -0.2814952f),
            new Vector2(0.1586419f, -0.5188532f),
            new Vector2(-0.05778766f, -0.7077841f),
            new Vector2(-0.4899519f, -0.8047636f),
            new Vector2(-0.02114609f, -0.8255388f),
            new Vector2(0.3538362f, -0.6984227f),
            new Vector2(0.468684f, -0.3411185f),
            new Vector2(0.3684494f, 0.06580618f),
            new Vector2(0.02376658f, 0.4493866f)
        };
        colliderShapes[4] = colliderShapes[3];
        colliderShapes[5] = new Vector2[]
        {
            new Vector2(-0.6131981f, -0.1019547f),
            new Vector2(-0.6170515f, -0.1001165f),
            new Vector2(-0.6246028f, -0.09046727f)
        };
    }
}
