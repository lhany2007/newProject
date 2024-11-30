using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    protected IWeaponAttackStrategy attackStrategy;

    protected WeaponData.Stats stats;
    protected WeaponData.WeaponInfo weaponInfo;

    [SerializeField] protected PolygonCollider2D weaponCollider;
    [SerializeField] protected GameObject weapon;

    protected Animator animator;
    private bool isAttacking = false;

    protected virtual void Start()
    {
        InitializeComponents();
        InitializeStrategy();
        UpdateCollider(0); // 기본 콜라이더 모양 설정
    }

    protected virtual void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        stats = WeaponManager.Instance.GetWeaponStats(weapon.tag);
        weaponInfo = WeaponManager.Instance.GetWeaponInfo(weapon.tag);
    }

    protected virtual void InitializeStrategy()
    {
        attackStrategy = WeaponManager.Instance.GetWeaponStrategy(weapon.tag);
        attackStrategy?.Initialize(weaponCollider, stats, weaponInfo);
    }

    protected virtual void UpdateCollider(int shapeIndex)
    {
        attackStrategy?.TransformCollider(shapeIndex);
    }

    private void Update()
    {
        string aniName = AnimationParams.Player.Attack;
        // 애니메이터의 현재 상태를 확인
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(aniName))
        {
            isAttacking = true;
            int frameIndex = GetCurrentFrameIndex();
            UpdateCollider(frameIndex);
        }
        else
        {
            if (isAttacking)
            {
                isAttacking = false;
                UpdateCollider(0); // Default 상태
            }
        }
    }

    // 현재 애니메이션 프레임 인덱스를 반환하는 함수
    private int GetCurrentFrameIndex()
    {
        float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        int frameIndex = Mathf.Clamp(Mathf.FloorToInt(normalizedTime * 6), 0, 5);
        return frameIndex;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(Layers.Monster))
        {
            attackStrategy?.ApplyDamage(other, stats.Damage);
        }
    }
}