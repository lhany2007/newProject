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
        // ShowGameOverUI(); // ���� ���� UI ǥ�� �Լ�
    }
}
