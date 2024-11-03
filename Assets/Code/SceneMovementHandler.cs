using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneMovementHandler : MonoBehaviour
{
    Button startButton;

    [Header("현재 씬 번호와 이 변수를 더해 이동할 씬 번호를 생성")]
    public int NextSceneIndex;

    void Start()
    {
        startButton = GetComponent<Button>();
        startButton.onClick.AddListener(() => OnButtonClick());
    }

    void OnButtonClick()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneMovement.Instance.NumMovement(currentSceneIndex + NextSceneIndex);
    }
}
