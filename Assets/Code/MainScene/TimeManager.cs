using UnityEngine;
using System;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public float GameTime = 0f;
    public float NextTierTime = 180f; // 다음 티어까지의 시간
    public int CurrentTier = 0; // 현재 경험치 구슬의 티어

    public UnityEvent<int> onTierChange; // 티어가 변경될 때 발생하는 이벤트

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

        if (GameTime >= NextTierTime) // 티어 변경 조건
        {
            CurrentTier = Mathf.Min(CurrentTier + 1, 5); // 최대 5티어까지
            GameTime = 0f;
            onTierChange.Invoke(CurrentTier); // 티어 변경 이벤트 호출
        }
    }

    // 다음 티어까지 남은 시간을 반환하는 함수
    public float GetTimeUntilNextTier()
    {
        return NextTierTime - GameTime;
    }
}
