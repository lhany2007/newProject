using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMovement : MonoBehaviour
{
    public static SceneMovement Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Scene의 번호로 이동
    /// </summary>
    /// <param name="num"></param>
    public void NumMovement(int num)
    {
        SceneManager.LoadScene(num);
    }

    /// <summary>
    /// Scene의 이름으로 이동
    /// </summary>
    /// <param name="name"></param>
    public void NameMovement(string name)
    {
        SceneManager.LoadScene(name);
    }
}
