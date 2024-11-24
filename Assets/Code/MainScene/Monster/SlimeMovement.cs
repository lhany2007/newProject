using UnityEngine;
using System.Collections;

public class SlimeMovement : MonoBehaviour
{
    Transform player;
    Animator animator;

    float speed;
    float waitTime = 1f;
    float moveDuration = 1f; // �̵��ð�

    float clipLength;

    const string IS_MOVING = "IsMoving";
    const string TARGET_CLIP_NAME = "SlimeMove";

    bool isMoving;

    void Start()
    {
        animator = GetComponent<Animator>();
        speed = MonsterManager.Instance.monsterStats.stats.Speed(MonsterManager.Instance.GetMonsterNameIndex("Bat"));
        isMoving = false;
        player = MonsterManager.Instance.monsterMovement.TargetPlayer;

        StartCoroutine(MoveRoutine()); // �̵� ����
    }

    void Update()
    {
        animator.SetBool(IS_MOVING, isMoving);
    }

    void Move()
    {
        transform.position = MonsterManager.Instance.monsterMovement.MoveToPlayer(speed, transform);
    }

    IEnumerator MoveRoutine()
    {
        while (player != null)
        {
            isMoving = true;

            float elapsedTime = 0f;
            while (elapsedTime < moveDuration)
            {
                Move();
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // �̵� �� ����
            isMoving = false;
            yield return new WaitForSeconds(waitTime);
        }
    }
}
