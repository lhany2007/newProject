using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack Instance;

    public PolygonCollider2D SwordCollider;

    const string MONSTER_LAYER = "Monster";
    
    bool isAttacking = false;

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
        colliderShapes = PlayerSwordCollider.Instance.ColliderShapes;

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
}
