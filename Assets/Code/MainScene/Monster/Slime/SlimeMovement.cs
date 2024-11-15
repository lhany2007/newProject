using System.Collections;
using UnityEngine;

public class SlimeMovement : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 2f;
    public float waitTime = 1f;

    Transform player;
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Player 태그를 가진 객체를 찾아서 Transform에 할당
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player 태그를 가진 객체를 찾을 수 없습니다.");
        }

        // 이동 루틴 시작
        if (player != null)
        {
            StartCoroutine(MoveRoutine());
        }
    }

    IEnumerator MoveRoutine()
    {
        while (player != null)
        {
            // 이동 애니메이션 재생
            animator.Play("SlimeMove");

            Vector2 startPosition = transform.position;
            Vector2 targetPosition = Vector2.MoveTowards(startPosition, player.position, moveDistance);

            float elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Idle 애니메이션 재생 및 대기
            animator.Play("Slime-idle");
            yield return new WaitForSeconds(waitTime);
        }
    }
}
