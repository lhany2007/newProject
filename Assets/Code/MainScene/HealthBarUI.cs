using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public static HealthBarUI Instance;

    public Transform player;
    public Vector3 offset;    // 플레이어와의 거리 오프셋 설정 (y값 조정)

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
            // 플레이어의 화면 좌표로 변환
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(player.position + offset);
            transform.position = screenPosition;
        }
    }

    // HP 값 업데이트 함수
    public void SetHealth(float health)
    {
        healthSlider.value = health;
    }
}
