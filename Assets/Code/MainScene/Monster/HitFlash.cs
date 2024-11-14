using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    public Color flashColor; // 피격 시 색상
    public float flashDuration = 0.1f; // 피격 색상 지속 시간

    SpriteRenderer spriteRenderer;
    Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // 원래 색상 저장
    }

    public void TriggerFlash()
    {
        StartCoroutine(FlashEffect());
    }

    IEnumerator FlashEffect()
    {
        spriteRenderer.color = flashColor; // 피격 시 색 변경
        yield return new WaitForSeconds(flashDuration); // 피격 색상 지속 시간
        spriteRenderer.color = originalColor; // 원래 색상으로 복귀
    }
}
