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
    /// Scene�� ��ȣ�� �̵�
    /// </summary>
    /// <param name="num"></param>
    public void NumMovement(int num)
    {
        SceneManager.LoadScene(num);
    }

    /// <summary>
    /// Scene�� �̸����� �̵�
    /// </summary>
    /// <param name="name"></param>
    public void NameMovement(string name)
    {
        SceneManager.LoadScene(name);
    }
}