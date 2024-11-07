using UnityEngine;
using UnityEngine.UI;

public class MonsterHealth : MonoBehaviour
{
    public Slider slider;
    public float maxHP = 100;

    float currentHP; // 현재 체력

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        slider.value = currentHP; // 슬라이더 값 업데이트

        // 체력이 0 이하이면 몬스터 오브젝트를 삭제
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
