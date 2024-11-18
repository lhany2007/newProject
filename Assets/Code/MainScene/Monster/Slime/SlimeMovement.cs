using System.Collections;
using UnityEngine;

public class SlimeMovement : MonoBehaviour
{
    float speed = 3f;
    float moveDistance = 2f;
    float waitTime = 0.5f;

    Transform player;
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Player 태그를 가진 객체를 찾아서 Transform에 할당 (몬스터가 프리펩이라 인스펙터에서 할당이 안됨)
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player 태그를 가진 객체를 찾을 수 없습니다.");
        }

        if (player != null)
        {
            StartCoroutine(MoveRoutine()); // 이동 시작
        }
    }

    IEnumerator MoveRoutine()
    {
        while (player != null)
        {
            animator.Play("SlimeMove"); // 이동 애니메이션 재생

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
