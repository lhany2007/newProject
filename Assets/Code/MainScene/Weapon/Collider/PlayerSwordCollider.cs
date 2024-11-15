using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordCollider : MonoBehaviour
{
    public static PlayerSwordCollider Instance;

    public Dictionary<int, Vector2[]> ColliderShapes;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ColliderShapes = new Dictionary<int, Vector2[]>(); // 딕셔너리 초기화
        InitializeColliderShapes(); // 콜라이더 데이터 초기화
    }

    public void InitializeColliderShapes()
    {
        ColliderShapes[0] = new Vector2[]
        {
            new Vector2(-0.6131981f, -0.1019547f),
            new Vector2(-0.6170515f, -0.1001165f),
            new Vector2(-0.6246028f, -0.09046727f)
        };
        ColliderShapes[1] = new Vector2[]
        {
            new Vector2(-0.6131981f, -0.1019547f),
            new Vector2(-0.6170515f, -0.1001165f),
            new Vector2(-0.6246028f, -0.09046727f)
        };
        ColliderShapes[2] = new Vector2[]
        {
            new Vector2(-0.6131981f, -0.1019547f),
            new Vector2(-0.6170515f, -0.1001165f),
            new Vector2(-0.6246028f, -0.09046727f)
        };
        ColliderShapes[3] = new Vector2[]
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
        ColliderShapes[4] = ColliderShapes[3];
        ColliderShapes[5] = new Vector2[]
        {
            new Vector2(-0.6131981f, -0.1019547f),
            new Vector2(-0.6170515f, -0.1001165f),
            new Vector2(-0.6246028f, -0.09046727f)
        };
    }
}
