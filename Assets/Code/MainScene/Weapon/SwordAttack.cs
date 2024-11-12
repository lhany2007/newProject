using UnityEngine;
using System.Collections.Generic;

public class SwordAttack : MonoBehaviour
{
    public static SwordAttack Instance;

    public Animator Ani;
    public PolygonCollider2D SwordCollider;

    Dictionary<int, Vector2[]> colliderShapes;

    const string MONSTER_TAG = "Monster";
    private bool isAttacking = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        Ani = GetComponent<Animator>();
        SwordCollider = GetComponent<PolygonCollider2D>();
        colliderShapes = new Dictionary<int, Vector2[]>();

        // 콜라이더 데이터 초기화
        InitializeColliderShapes();

        // 기본 콜라이더 모양 설정
        UpdateColliderShape(0);
    }

    void InitializeColliderShapes()
    {
        colliderShapes[0] = new Vector2[]
        {
             new Vector2(-0.6094514f , 0.01571336f),
             new Vector2(-0.6433504f, 0.003350556f),
             new Vector2(-0.6518243f, -0.02694057f),
             new Vector2(-0.747909f, 0.01453136f),
             new Vector2(-0.7736799f, -0.08115555f),
             new Vector2(-0.8388443f, -0.1554939f),
             new Vector2(-0.8623222f, -0.1985957f),
             new Vector2(-0.9418489f, -0.2851469f),
             new Vector2(-0.9797805f, -0.3633506f),
             new Vector2(-0.9096707f, -0.3707682f),
             new Vector2(-0.8233883f, -0.3044553f),
             new Vector2(-0.7618965f, -0.2466206f),
             new Vector2(-0.7186829f, -0.2466206f),
             new Vector2(-0.6240093f, -0.2093233f),
             new Vector2(-0.6090122f, -0.12f),
             new Vector2(-0.6708261f, -0.1002195f),
             new Vector2(-0.6268284f,- 0.04990893f),
             new Vector2(-0.5867591f, -0.01934158f)
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
        // 애니메이터의 현재 상태를 확인
        if (Ani.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
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
        float normalizedTime = Ani.GetCurrentAnimatorStateInfo(0).normalizedTime;
        int frameIndex = Mathf.Clamp(Mathf.FloorToInt(normalizedTime * 6), 0, 5);
        return frameIndex;
    }

    // 콜라이더 모양 업데이트 함수
    private void UpdateColliderShape(int shapeIndex)
    {
        if (colliderShapes.ContainsKey(shapeIndex))
        {
            SwordCollider.SetPath(0, colliderShapes[shapeIndex]);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(MONSTER_TAG))
        {
            MonsterHealth monsterHealth = other.GetComponent<MonsterHealth>();
            if (monsterHealth != null)
            {
                monsterHealth.TakeDamage(WeaponManager.Instance.WeaponDamageDictionary["Sword"]);
            }
        }
    }
}
