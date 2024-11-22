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

    public void GameOver()
    {
        Time.timeScale = 0f;
        // ShowGameOverUI(); // 게임 오버 UI 표시 함수
    }
}
public static class Tags
{
    public const string EXP_TAG = "Exp";

    public const string MONSTER_LAYER = "Monster";
}