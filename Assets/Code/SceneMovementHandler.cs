using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneMovementHandler : MonoBehaviour
{
    Button startButton;

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
