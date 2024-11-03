using UnityEngine;

public class TextPulseEffect : MonoBehaviour
{
    const float SIZE = 0.15f;
    const float UP_SIZE_TIME = 2.8f; // 3초 동안 커짐
    const float DOWN_SIZE_TIME = 2.8f; // 3초 동안 작아짐

    float time = 0f;

    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        float cycleTime = UP_SIZE_TIME + DOWN_SIZE_TIME; // 전체 사이클 시간
        time += Time.deltaTime;

        // 주기적으로 time을 0에서 cycleTime 사이에서 반복되도록 함
        if (time > cycleTime)
        {
            time -= cycleTime;
        }

        // 커지는 애니매이션
        if (time <= UP_SIZE_TIME)
        {
            rectTransform.localScale = Vector3.one * (1 + SIZE * (time / UP_SIZE_TIME));
        }
        // 작아지는 애니매이션
        else
        {
            float downTime = time - UP_SIZE_TIME;
            rectTransform.localScale = Vector3.one * (1 + SIZE * (1 - (downTime / DOWN_SIZE_TIME)));
        }
    }
}
