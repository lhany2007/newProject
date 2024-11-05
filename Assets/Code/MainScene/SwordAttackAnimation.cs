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

    // SwordAttack �ִϸ��̼��� ���� �� ȣ��Ǵ� �Լ�
    public void OnSwordAttackEnd()
    {
        Ani.SetBool("isReturning", true); // SwordAttack���� Default�� ���ư��� ���� ����
    }

    // Default ���¿� ������ �� ȣ��Ǿ� isReturning�� �ʱ�ȭ
    public void ResetIsReturning()
    {
        Ani.SetBool("isReturning", false);
    }
}
