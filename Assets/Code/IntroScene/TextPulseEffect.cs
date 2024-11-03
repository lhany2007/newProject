using UnityEngine;

public class TextPulseEffect : MonoBehaviour
{
    const float SIZE = 0.15f;
    const float UP_SIZE_TIME = 2.8f; // 3�� ���� Ŀ��
    const float DOWN_SIZE_TIME = 2.8f; // 3�� ���� �۾���

    float time = 0f;

    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        float cycleTime = UP_SIZE_TIME + DOWN_SIZE_TIME; // ��ü ����Ŭ �ð�
        time += Time.deltaTime;

        // �ֱ������� time�� 0���� cycleTime ���̿��� �ݺ��ǵ��� ��
        if (time > cycleTime)
        {
            time -= cycleTime;
        }

        // Ŀ���� �ִϸ��̼�
        if (time <= UP_SIZE_TIME)
        {
            rectTransform.localScale = Vector3.one * (1 + SIZE * (time / UP_SIZE_TIME));
        }
        // �۾����� �ִϸ��̼�
        else
        {
            float downTime = time - UP_SIZE_TIME;
            rectTransform.localScale = Vector3.one * (1 + SIZE * (1 - (downTime / DOWN_SIZE_TIME)));
        }
    }
}
