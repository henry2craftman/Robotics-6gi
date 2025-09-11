using UnityEngine;

// 스크립트 추가 시 필요한 컴포넌트를 자동으로 추가해주는 어트리뷰트
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("이동 속도")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;

    [Header("회전 속도")]
    public float rotationSpeed = 10.0f;

    [Header("중력")]
    public float gravity = -9.81f;

    // 주요 컴포넌트 참조
    private CharacterController characterController;
    private Animator animator;

    // 내부 계산용 변수
    private Vector3 moveDirection;
    private float velocityY = 0f; // y축(수직) 속도

    void Start()
    {
        // 컴포넌트들을 시작할 때 한 번만 가져와서 저장 (효율적)
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. 사용자 입력 받기 (W, A, S, D 또는 방향키)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 입력 값을 기반으로 이동 방향 벡터 생성
        Vector3 inputDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // 2. 달리기 상태 확인 (왼쪽 Shift 키)
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // 3. 실제 이동 속도 결정 (걷기 또는 달리기)
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // 이동 입력이 없으면 속도를 0으로 설정
        if (inputDirection.magnitude < 0.1f)
        {
            currentSpeed = 0f;
        }

        // 4. 이동 및 회전 처리
        if (inputDirection.magnitude >= 0.1f)
        {
            // 목표 회전값 계산
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);

            // 현재 회전값에서 목표 회전값으로 부드럽게 회전 (Slerp)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 이동 방향 설정
            moveDirection = inputDirection * currentSpeed;
        }
        else
        {
            // 이동 입력이 없으면 수평 이동은 멈춤
            moveDirection = Vector3.zero;
        }

        // 5. 중력 적용
        // 캐릭터가 땅에 닿아있으면 수직 속도 리셋
        if (characterController.isGrounded && velocityY < 0)
        {
            velocityY = -2f; // 땅에 안정적으로 붙어있도록 살짝 아래로 힘을 줌
        }
        // 중력 가속도 적용
        velocityY += gravity * Time.deltaTime;
        moveDirection.y = velocityY;

        // 6. CharacterController를 이용해 최종 이동 적용
        characterController.Move(moveDirection * Time.deltaTime);

        // 7. 애니메이터 파라미터 업데이트
        // 애니메이션 상태를 결정할 값을 계산 (0: 멈춤, 0.5: 걷기, 1: 달리기)
        float animationSpeedPercent = 0f;
        if (currentSpeed > 0)
        {
            animationSpeedPercent = isRunning ? 1f : 0.5f;
        }

        // Animator의 moveSpeed 파라미터에 계산된 값을 전달
        // 0.1f의 dampTime을 주어 애니메이션 전환이 부드럽게 되도록 함
        animator.SetFloat("moveSpeed", animationSpeedPercent, 0.1f, Time.deltaTime);
    }
}