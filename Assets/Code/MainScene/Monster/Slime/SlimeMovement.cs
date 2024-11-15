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
        // Player �±׸� ���� ��ü�� ã�Ƽ� Transform�� �Ҵ�
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player �±׸� ���� ��ü�� ã�� �� �����ϴ�.");
        }

        // �̵� ��ƾ ����
        if (player != null)
        {
            StartCoroutine(MoveRoutine());
        }
    }

    IEnumerator MoveRoutine()
    {
        while (player != null)
        {
            // �̵� �ִϸ��̼� ���
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

            // Idle �ִϸ��̼� ��� �� ���
            animator.Play("Slime-idle");
            yield return new WaitForSeconds(waitTime);
        }
    }
}
