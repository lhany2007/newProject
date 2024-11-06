using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public static HealthBarUI Instance;

    public Transform player;
    public Vector3 offset;    // �÷��̾���� �Ÿ� ������ ���� (y�� ����)

    Slider healthSlider;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        healthSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (player != null)
        {
            // �÷��̾��� ȭ�� ��ǥ�� ��ȯ
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(player.position + offset);
            transform.position = screenPosition;
        }
    }

    // HP �� ������Ʈ �Լ�
    public void SetHealth(float health)
    {
        healthSlider.value = health;
    }
}
