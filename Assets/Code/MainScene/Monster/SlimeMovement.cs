using UnityEngine;
using System.Collections;

public class SlimeMovement : MonoBehaviour
{
    float speed = 3f;
    float moveDistance = 3f;
    float waitTime = 1f;
    float moveDuration = 1f; // �̵��ð�

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

        // Player �±׸� ���� ��ü�� ã�� Transform�� �Ҵ�
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player �±׸� ���� ��ü�� ã�� �� �����ϴ�.");
        }

        if (player != null)
        {
            StartCoroutine(MoveRoutine()); // �̵� ����
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

            // �̵� ����
            Vector2 startPosition = transform.position;
            Vector2 targetPosition = Vector2.MoveTowards(startPosition, player.position, moveDistance);

            float elapsedTime = 0f;
            while (elapsedTime < moveDuration)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // �̵� �� ����
            isMoving = false;
            animator.speed = 1f; // �ִϸ��̼� �ӵ� �ʱ�ȭ
            yield return new WaitForSeconds(waitTime);
        }
    }
}
