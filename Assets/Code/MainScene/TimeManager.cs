using UnityEngine;
using System;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public float gameTime = 0f;
    public float nextTierTime = 180f; // ���� Ƽ������� �ð�
    public int currentTier = 0; // ���� ����ġ ������ Ƽ��

    public UnityEvent<int> onTierChange; // Ƽ� ����� �� �߻��ϴ� �̺�Ʈ

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (onTierChange == null)
        {
            onTierChange = new UnityEvent<int>();
        }
    }

    void Update()
    {
        gameTime += Time.deltaTime;

        if (gameTime >= nextTierTime) // Ƽ�� ���� ����
        {
            currentTier = Mathf.Min(currentTier + 1, 5); // �ִ� 5Ƽ�����
            gameTime = 0f;
            onTierChange.Invoke(currentTier); // Ƽ�� ���� �̺�Ʈ ȣ��
        }
    }

    // ���� Ƽ����� ���� �ð��� ��ȯ�ϴ� �Լ�
    public float GetTimeUntilNextTier()
    {
        return nextTierTime - gameTime;
    }
}
