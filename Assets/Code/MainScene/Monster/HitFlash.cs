using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    public Color flashColor; // �ǰ� �� ����
    public float flashDuration = 0.1f; // �ǰ� ���� ���� �ð�

    SpriteRenderer spriteRenderer;
    Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // ���� ���� ����
    }

    public void TriggerFlash()
    {
        StartCoroutine(FlashEffect());
    }

    IEnumerator FlashEffect()
    {
        spriteRenderer.color = flashColor; // �ǰ� �� �� ����
        yield return new WaitForSeconds(flashDuration); // �ǰ� ���� ���� �ð�
        spriteRenderer.color = originalColor; // ���� �������� ����
    }
}
