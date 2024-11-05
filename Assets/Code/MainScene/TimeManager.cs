using UnityEngine;
using System;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public float GameTime = 0f;
    public float NextTierTime = 180f; // ���� Ƽ������� �ð�
    public int CurrentTier = 0; // ���� ����ġ ������ Ƽ��

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
        GameTime += Time.deltaTime;

        if (GameTime >= NextTierTime) // Ƽ�� ���� ����
        {
            CurrentTier = Mathf.Min(CurrentTier + 1, 5); // �ִ� 5Ƽ�����
            GameTime = 0f;
            onTierChange.Invoke(CurrentTier); // Ƽ�� ���� �̺�Ʈ ȣ��
        }
    }

    // ���� Ƽ����� ���� �ð��� ��ȯ�ϴ� �Լ�
    public float GetTimeUntilNextTier()
    {
        return NextTierTime - GameTime;
    }
}
