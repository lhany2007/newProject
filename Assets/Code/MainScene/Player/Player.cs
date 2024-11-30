using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerAnimation))]

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerAnimation animation;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        animation = GetComponent<PlayerAnimation>();
    }
}