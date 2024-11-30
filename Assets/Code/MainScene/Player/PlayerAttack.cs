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
        UpdateCollider(0); // �⺻ �ݶ��̴� ��� ����
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
        // �ִϸ������� ���� ���¸� Ȯ��
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
                UpdateCollider(0); // Default ����
            }
        }
    }

    // ���� �ִϸ��̼� ������ �ε����� ��ȯ�ϴ� �Լ�
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