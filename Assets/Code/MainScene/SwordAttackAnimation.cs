using UnityEngine;

public class SwordAttackAnimation : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 마우스 왼쪽 버튼을 클릭했을 때 공격 애니메이션 트리거
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("isMouseClicked");
        }
    }

    // SwordAttack 애니메이션이 끝날 때 호출되는 함수
    public void OnSwordAttackEnd()
    {
        animator.SetBool("isReturning", true); // SwordAttack에서 Default로 돌아가는 조건 설정
    }

    // Default 상태에 진입할 때 호출되어 isReturning을 초기화
    public void ResetIsReturning()
    {
        animator.SetBool("isReturning", false);
    }
}
