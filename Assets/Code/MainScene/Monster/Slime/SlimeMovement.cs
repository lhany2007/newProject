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
        // Player �±׸� ���� ��ü�� ã�Ƽ� Transform�� �Ҵ� (���Ͱ� �������̶� �ν����Ϳ��� �Ҵ��� �ȵ�)
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

    IEnumerator MoveRoutine()
    {
        while (player != null)
        {
            animator.Play("SlimeMove"); // �̵� �ִϸ��̼� ���

            Vector2 startPosition = transform.position;
            Vector2 targetPosition = Vector2.MoveTowards(startPosition, player.position, moveDistance);

            float elapsedTime = 0f;
            while (elapsedTime < 1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Idle �ִϸ��̼� ��� �� ���
            animator.Play("Slime-idle");
            yield return new WaitForSeconds(waitTime);
        }
    }
}
