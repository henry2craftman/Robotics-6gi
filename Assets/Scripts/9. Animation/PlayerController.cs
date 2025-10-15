using UnityEngine;

namespace Animation
{
    
// ��ũ��Ʈ �߰� �� �ʿ��� ������Ʈ�� �ڵ����� �߰����ִ� ��Ʈ����Ʈ
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("�̵� �ӵ�")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;

    [Header("ȸ�� �ӵ�")]
    public float rotationSpeed = 10.0f;

    [Header("�߷�")]
    public float gravity = -9.81f;

    // �ֿ� ������Ʈ ����
    private CharacterController characterController;
    private Animator animator;

    // ���� ���� ����
    private Vector3 moveDirection;
    private float velocityY = 0f; // y��(����) �ӵ�

    void Start()
    {
        // ������Ʈ���� ������ �� �� ���� �����ͼ� ���� (ȿ����)
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. ����� �Է� �ޱ� (W, A, S, D �Ǵ� ����Ű)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // �Է� ���� ������� �̵� ���� ���� ����
        Vector3 inputDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // 2. �޸��� ���� Ȯ�� (���� Shift Ű)
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // 3. ���� �̵� �ӵ� ���� (�ȱ� �Ǵ� �޸���)
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // �̵� �Է��� ������ �ӵ��� 0���� ����
        if (inputDirection.magnitude < 0.1f)
        {
            currentSpeed = 0f;
        }

        // 4. �̵� �� ȸ�� ó��
        if (inputDirection.magnitude >= 0.1f)
        {
            // ��ǥ ȸ���� ���
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);

            // ���� ȸ�������� ��ǥ ȸ�������� �ε巴�� ȸ�� (Slerp)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // �̵� ���� ����
            moveDirection = inputDirection * currentSpeed;
        }
        else
        {
            // �̵� �Է��� ������ ���� �̵��� ����
            moveDirection = Vector3.zero;
        }

        // 5. �߷� ����
        // ĳ���Ͱ� ���� ��������� ���� �ӵ� ����
        if (characterController.isGrounded && velocityY < 0)
        {
            velocityY = -2f; // ���� ���������� �پ��ֵ��� ��¦ �Ʒ��� ���� ��
        }
        // �߷� ���ӵ� ����
        velocityY += gravity * Time.deltaTime;
        moveDirection.y = velocityY;

        // 6. CharacterController�� �̿��� ���� �̵� ����
        characterController.Move(moveDirection * Time.deltaTime);

        // 7. �ִϸ����� �Ķ���� ������Ʈ
        // �ִϸ��̼� ���¸� ������ ���� ��� (0: ����, 0.5: �ȱ�, 1: �޸���)
        float animationSpeedPercent = 0f;
        if (currentSpeed > 0)
        {
            animationSpeedPercent = isRunning ? 1f : 0.5f;
        }

        // Animator�� moveSpeed �Ķ���Ϳ� ���� ���� ����
        // 0.1f�� dampTime�� �־� �ִϸ��̼� ��ȯ�� �ε巴�� �ǵ��� ��
        animator.SetFloat("moveSpeed", animationSpeedPercent, 0.1f, Time.deltaTime);
    }
}
}
