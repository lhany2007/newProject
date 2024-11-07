using UnityEngine;
using UnityEngine.UI;

public class MonsterHealth : MonoBehaviour
{
    public Slider slider;
    public float maxHP = 100;

    float currentHP; // ���� ü��

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        slider.value = currentHP; // �����̴� �� ������Ʈ

        // ü���� 0 �����̸� ���� ������Ʈ�� ����
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
