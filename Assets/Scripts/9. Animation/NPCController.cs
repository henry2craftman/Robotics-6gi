using UnityEngine;

// NPC의 상태를 나타내는 열거형(Enum)
public enum NPCStateNoNav { Idle, Patrol, Chase, Attack }

public class NPCController : MonoBehaviour
{
    [Header("상태")]
    public NPCStateNoNav currentState;

    [Header("주요 대상")]
    private Transform player;
    private Animator animator;

    [Header("이동 및 회전")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float rotationSpeed = 5f; // 초당 회전 속도

    [Header("순찰 (Patrol) 설정")]
    public Transform[] waypoints; // 순찰 지점들
    private int currentWaypointIndex = 0;
    private Vector3 currentTargetPosition;

    [Header("대기 (Idle) 설정")]
    public float idleTime = 2f; // 대기 시간
    private float idleTimer;

    [Header("감지 및 공격 범위")]
    public float detectionRange = 15f;
    public float attackRange = 2f;

    [Header("공격 (Attack) 설정")]
    public float attackCooldown = 2f; // 공격 쿨타임
    private float lastAttackTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 초기 상태 설정
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
                // 목표 지점으로 이동 및 회전
                MoveAndRotate(currentTargetPosition, patrolSpeed);

                if (distanceToPlayer <= detectionRange)
                {
                    ChangeState(NPCStateNoNav.Chase);
                }
                // 목표 지점에 거의 도착했으면
                else if (Vector3.Distance(transform.position, currentTargetPosition) < 1.0f)
                {
                    ChangeState(NPCStateNoNav.Idle);
                }
                break;

            case NPCStateNoNav.Chase:
                // 플레이어를 향해 이동 및 회전
                MoveAndRotate(player.position, chaseSpeed);

                if (distanceToPlayer <= attackRange)
                {
                    ChangeState(NPCStateNoNav.Attack);
                }
                else if (distanceToPlayer > detectionRange)
                {
                    // 플레이어를 놓쳤을 때 바로 순찰 상태로 돌아가도록 설정
                    ChangeState(NPCStateNoNav.Patrol);
                }
                break;

            case NPCStateNoNav.Attack:
                // 공격 범위 밖으로 나가면 다시 추격
                if (distanceToPlayer > attackRange)
                {
                    ChangeState(NPCStateNoNav.Chase);
                }
                else
                {
                    // 공격 시에는 이동은 멈추고 플레이어를 바라보기만 함
                    RotateTowards(player.position);

                    if (Time.time >= lastAttackTime + attackCooldown)
                    {
                        lastAttackTime = Time.time;
                        animator.SetTrigger("Attack");
                        Debug.Log("공격!");
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

    // 목표 지점을 향해 회전하는 함수
    private void RotateTowards(Vector3 targetPosition)
    {
        // 목표 지점까지의 방향 벡터 계산 (y축은 무시하여 수평으로만 회전)
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        // 목표 방향을 바라보는 Quaternion 값 생성
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 현재 각도에서 목표 각도까지 부드럽게 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // 목표 지점으로 이동하고 회전하는 함수
    private void MoveAndRotate(Vector3 targetPosition, float speed)
    {
        // 먼저 목표를 향해 회전
        //RotateTowards(targetPosition);

        // 캐릭터의 앞 방향으로 이동
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    // Scene 뷰에서 감지 범위와 공격 범위를 시각적으로 보여주는 기능
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // 감지 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // 공격 범위
    }
}