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
        // ���콺 ���� ��ư�� Ŭ������ �� ���� �ִϸ��̼� Ʈ����
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("isMouseClicked");
        }
    }

    // SwordAttack �ִϸ��̼��� ���� �� ȣ��Ǵ� �Լ�
    public void OnSwordAttackEnd()
    {
        animator.SetBool("isReturning", true); // SwordAttack���� Default�� ���ư��� ���� ����
    }

    // Default ���¿� ������ �� ȣ��Ǿ� isReturning�� �ʱ�ȭ
    public void ResetIsReturning()
    {
        animator.SetBool("isReturning", false);
    }
}
