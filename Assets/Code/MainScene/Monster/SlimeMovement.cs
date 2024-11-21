using UnityEngine;
using System.Collections;

public class SlimeMovement : MonoBehaviour
{
    float speed = 3f;
    float moveDistance = 3f;
    float waitTime = 1f;
    float moveDuration = 1f; // 이동시간

    float clipLength;

    const string IS_MOVING = "IsMoving";
    const string TARGET_CLIP_NAME = "SlimeMove";

    Transform player;
    Animator animator;

    bool isMoving;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        isMoving = false;

        // Player 태그를 가진 객체를 찾아 Transform에 할당
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

    void Update()
    {
        animator.SetBool(IS_MOVING, isMoving);
    }

    IEnumerator MoveRoutine()
    {
        while (player != null)
        {
            isMoving = true;

            // 이동 시작
            Vector2 startPosition = transform.position;
            Vector2 targetPosition = Vector2.MoveTowards(startPosition, player.position, moveDistance);

            float elapsedTime = 0f;
            while (elapsedTime < moveDuration)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 이동 후 정지
            isMoving = false;
            animator.speed = 1f; // 애니메이션 속도 초기화
            yield return new WaitForSeconds(waitTime);
        }
    }
}
