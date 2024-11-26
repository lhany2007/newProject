using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StopAnimation(Animator animator)
    {
        animator.enabled = false;
    }

    public void StartAnimation(Animator animator)
    {
        animator.enabled = true;
    }
}