using UnityEngine;

public class SwordAttackAnimation : MonoBehaviour
{
    public static SwordAttackAnimation Instance;

    public Animator Ani;

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
    }

    // SwordAttack 애니메이션이 끝날 때 호출되는 함수
    public void OnSwordAttackEnd()
    {
        Ani.SetBool("isReturning", true); // SwordAttack에서 Default로 돌아가는 조건 설정
    }

    // Default 상태에 진입할 때 호출되어 isReturning을 초기화
    public void ResetIsReturning()
    {
        Ani.SetBool("isReturning", false);
    }
}
