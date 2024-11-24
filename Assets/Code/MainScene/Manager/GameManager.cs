using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Knockback(Rigidbody2D rb, float knockbackForce, Vector3 pos, Vector3 targetVec, Animator animator)
    {
        StopMoving(rb, animator);
        rb.AddForce((pos - targetVec).normalized * knockbackForce, ForceMode2D.Impulse);
        StartAnimation(animator);
    }

    public void StopMoving(Rigidbody2D rb, Animator animator)
    {
        rb.linearVelocity = Vector2.zero;
        animator.enabled = false;
    }

    public void StartAnimation(Animator animator)
    {
        animator.enabled = true;
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        // ShowGameOverUI(); // ���� ���� UI ǥ�� �Լ�
    }
}
public static class Tags
{
    public const string EXP_TAG = "Exp";
    public const string PLAYER = "Player";

    public const string MONSTER_LAYER = "Monster";
}