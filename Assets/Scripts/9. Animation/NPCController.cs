using UnityEngine;

// NPC�� ���¸� ��Ÿ���� ������(Enum)
public enum NPCStateNoNav { Idle, Patrol, Chase, Attack }

public class NPCController : MonoBehaviour
{
    [Header("����")]
    public NPCStateNoNav currentState;

    [Header("�ֿ� ���")]
    private Transform player;
    private Animator animator;

    [Header("�̵� �� ȸ��")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float rotationSpeed = 5f; // �ʴ� ȸ�� �ӵ�

    [Header("���� (Patrol) ����")]
    public Transform[] waypoints; // ���� ������
    private int currentWaypointIndex = 0;
    private Vector3 currentTargetPosition;

    [Header("��� (Idle) ����")]
    public float idleTime = 2f; // ��� �ð�
    private float idleTimer;

    [Header("���� �� ���� ����")]
    public float detectionRange = 15f;
    public float attackRange = 2f;

    [Header("���� (Attack) ����")]
    public float attackCooldown = 2f; // ���� ��Ÿ��
    private float lastAttackTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // �ʱ� ���� ����
        ChangeState(NPCStateNoNav.Idle);
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case NPCStateNoNav.Idle:
                idleTimer -= Time.deltaTime;

                if (distanceToPlayer <= detectionRange)
                {
                    ChangeState(NPCStateNoNav.Chase);
                }
                else if (idleTimer <= 0f)
                {
                    ChangeState(NPCStateNoNav.Patrol);
                }
                break;

            case NPCStateNoNav.Patrol:
                // ��ǥ �������� �̵� �� ȸ��
                MoveAndRotate(currentTargetPosition, patrolSpeed);

                if (distanceToPlayer <= detectionRange)
                {
                    ChangeState(NPCStateNoNav.Chase);
                }
                // ��ǥ ������ ���� ����������
                else if (Vector3.Distance(transform.position, currentTargetPosition) < 1.0f)
                {
                    ChangeState(NPCStateNoNav.Idle);
                }
                break;

            case NPCStateNoNav.Chase:
                // �÷��̾ ���� �̵� �� ȸ��
                MoveAndRotate(player.position, chaseSpeed);

                if (distanceToPlayer <= attackRange)
                {
                    ChangeState(NPCStateNoNav.Attack);
                }
                else if (distanceToPlayer > detectionRange)
                {
                    // �÷��̾ ������ �� �ٷ� ���� ���·� ���ư����� ����
                    ChangeState(NPCStateNoNav.Patrol);
                }
                break;

            case NPCStateNoNav.Attack:
                // ���� ���� ������ ������ �ٽ� �߰�
                if (distanceToPlayer > attackRange)
                {
                    ChangeState(NPCStateNoNav.Chase);
                }
                else
                {
                    // ���� �ÿ��� �̵��� ���߰� �÷��̾ �ٶ󺸱⸸ ��
                    RotateTowards(player.position);

                    if (Time.time >= lastAttackTime + attackCooldown)
                    {
                        lastAttackTime = Time.time;
                        animator.SetTrigger("Attack");
                        Debug.Log("����!");
                    }
                }
                break;
        }
    }

    private void ChangeState(NPCStateNoNav newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (currentState)
        {
            case NPCStateNoNav.Idle:
                idleTimer = idleTime;
                animator.SetBool("isWalking", false);
                animator.SetBool("isChasing", false);
                break;

            case NPCStateNoNav.Patrol:
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                currentTargetPosition = waypoints[currentWaypointIndex].position;
                animator.SetBool("isWalking", true);
                animator.SetBool("isChasing", false);
                break;

            case NPCStateNoNav.Chase:
                animator.SetBool("isWalking", false);
                animator.SetBool("isChasing", true);
                break;

            case NPCStateNoNav.Attack:
                animator.SetBool("isWalking", false);
                animator.SetBool("isChasing", false);
                break;
        }
    }

    // ��ǥ ������ ���� ȸ���ϴ� �Լ�
    private void RotateTowards(Vector3 targetPosition)
    {
        // ��ǥ ���������� ���� ���� ��� (y���� �����Ͽ� �������θ� ȸ��)
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        // ��ǥ ������ �ٶ󺸴� Quaternion �� ����
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // ���� �������� ��ǥ �������� �ε巴�� ȸ��
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // ��ǥ �������� �̵��ϰ� ȸ���ϴ� �Լ�
    private void MoveAndRotate(Vector3 targetPosition, float speed)
    {
        // ���� ��ǥ�� ���� ȸ��
        //RotateTowards(targetPosition);

        // ĳ������ �� �������� �̵�
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    // Scene �信�� ���� ������ ���� ������ �ð������� �����ִ� ���
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // ���� ����
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // ���� ����
    }
}