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
        colliderShapes = new Dictionary<int, Vector2[]>();

        // �ݶ��̴� ������ �ʱ�ȭ
        InitializeColliderShapes();

        // �⺻ �ݶ��̴� ��� ����
        UpdateColliderShape(0);
    }

    void InitializeColliderShapes()
    {
        colliderShapes[0] = new Vector2[]
        {
             new Vector2(-0.6131981f , -0.1019547f),
             new Vector2(-0.6170515f, -0.1001165f),
             new Vector2(-0.6246028f, -0.09046727f)
        };
        colliderShapes[1] = new Vector2[]
        {
             new Vector2(-0.76f, -0.2f),
             new Vector2(-0.6f, -0.2f),
             new Vector2(-0.7032069f, 0.1631274f),
             new Vector2(-0.7652284f, 0.4441631f),
             new Vector2(-0.8238641f, 0.801813f),
             new Vector2(-0.96f, 0.88f),
             new Vector2(-0.9762802f, 0.7053096f),
             new Vector2(-0.8552293f, 0.3378585f)
        };
        colliderShapes[2] = new Vector2[]
        {
             new Vector2(-0.9615639f, 0.8214843f),
             new Vector2(-0.8317733f, 0.4869122f),
             new Vector2(-0.7745721f, 0.3762947f),
             new Vector2(-0.84f, -0.16f),
             new Vector2(-0.72f, -0.2f),
             new Vector2(-0.6f, -0.16f),
             new Vector2(-0.6185156f, 0.33498f),
             new Vector2(-0.5832069f, 0.5995015f),
             new Vector2(-0.5519322f, 0.8558867f)
        };
        colliderShapes[3] = new Vector2[]
        {
             new Vector2(-0.1185073f, 0.8435436f),
             new Vector2(-0.8398141f, 0.8547323f),
             new Vector2(-0.2095764f, 0.6217886f),
             new Vector2(0.0720681f, 0.3196282f),
             new Vector2(-0.04257956f, -0.04276454f),
             new Vector2(-0.7208394f, -0.06629322f),
             new Vector2(-0.6991154f, -0.1376226f),
             new Vector2(-0.08270406f, -0.3625183f),
             new Vector2(0.4113747f, -0.5291378f),
             new Vector2(0.5747324f, -0.4974364f),
             new Vector2(0.7003267f, -0.2916563f),
             new Vector2(0.6984617f, 0.1962705f),
             new Vector2(0.3167378f, 0.604383f)
        };
        colliderShapes[4] = new Vector2[]
        {
             new Vector2(-0.0518114f, -0.3602977f),
             new Vector2(-0.5686354f, -0.1583123f),
             new Vector2(-0.7366749f, -0.0748383f),
             new Vector2(-0.6704718f, -0.2540942f),
             new Vector2(-0.5334992f, -0.6365263f),
             new Vector2(0.1991071f, -0.599107f),
             new Vector2(0.4189585f, -0.5997024f),
             new Vector2(0.4995534f, -0.5375682f),
             new Vector2(0.3530525f, -0.4802976f)
        };
        colliderShapes[5] = new Vector2[]
        {
             new Vector2(-0.7476411f, -0.1405839f),
             new Vector2(-0.7932367f, -0.2924428f),
             new Vector2(-0.859374f, -0.5517749f),
             new Vector2(-0.8220038f, -0.6194161f),
             new Vector2(-0.7171602f, -0.6538206f),
             new Vector2(-0.6854272f, -0.3917328f),
             new Vector2(-0.6744045f, -0.1378702f),
             new Vector2(-0.6910228f, -0.06889307f)
        };
    }

    void Update()
    {
        // �ִϸ������� ���� ���¸� Ȯ��
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
                UpdateColliderShape(0); // Default ����
            }
        }
    }

    // ���� �ִϸ��̼� ������ �ε����� ��ȯ�ϴ� �Լ�
    int GetCurrentFrameIndex()
    {
        float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        int frameIndex = Mathf.Clamp(Mathf.FloorToInt(normalizedTime * 6), 0, 5);
        return frameIndex;
    }

    // �ݶ��̴� ��� ������Ʈ �Լ�
    private void UpdateColliderShape(int shapeIndex)
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
