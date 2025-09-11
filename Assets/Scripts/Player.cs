using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    public float rotSpeed = 10;
    CharacterController controller;
    public float jumDuration = 0.1f;
    public float maxJumpForce = 1;
    public float jumpForce = 5;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;  // ���� ���� (�̰����� ���� ���� ����ϴ� ���� �� ������)
    public Transform nPC;
    private Vector3 playerVelocity; // �÷��̾��� ���� �ӵ��� ������ ����
    private bool isGrounded;

    // LifeCycle �Լ�
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("Player�� ������!");

        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move();
        //MoveByJoypad();

        RotateBody();

        isGrounded = controller.isGrounded;

        // ���� ���� �� Y�� �ӵ� �ʱ�ȭ (�߷� ���� ����)
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // ���� �پ��ֵ��� �ణ�� ���� �� ���� (�ɼ�)
        }

        MoveByCharacterController();
        ApplyJumpAndGravity();

        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    Instantiate(nPC);
        //}
    }

    private void RotateBody()
    {
        float mouseX = Input.GetAxis("Mouse X"); // -1 ~ 1 ������ ��

        //print($"Mouse Input: {mouseX}, {mouseY}");

        float dy = mouseX * rotSpeed * Time.deltaTime;

        transform.eulerAngles += new Vector3(0, dy, 0);
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //transform.position = transform.position + Vector3.left; // Vector3.left ������ǥ(������ǥ)�� ���� ����
            transform.position -= transform.right * moveSpeed * Time.deltaTime; // ������ǥ�� ���� ����
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            //transform.position = transform.position + Vector3.right; // Vector3.right ������ǥ�� ������ ���� ����
            transform.position += transform.right * moveSpeed * Time.deltaTime; // ������ǥ�� ������ ����
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime; // 0.03s -> 0.01
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position -= transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    // ������ �߰�
    private void MoveByJoypad()
    {
        float horizontal = Input.GetAxis("Horizontal"); // Ű���� ����, ������ -1 ~ 1�� ǥ��
        float vertical = Input.GetAxis("Vertical");     // Ű���� ����Ű ��, �Ʒ�

        Vector3 direction = (transform.forward * vertical) + (transform.right * horizontal);

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void MoveByCharacterController()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �̵� ���� ��� (��/��, ��/��)
        Vector3 moveDirection = (transform.forward * vertical) + (transform.right * horizontal);

        // �̵� (Y�� �ӵ� ����)
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
    private void ApplyJumpAndGravity()
    {
        // ���� �Է� ó��
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            // ���� ���̷κ��� �ʱ� ���� �ӵ� ��� ����:
            // v = sqrt(h * -2 * g)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // �߷� ����
        playerVelocity.y += gravity * Time.deltaTime;

        // ���� Y�� �ӵ��� CharacterController�� ����
        controller.Move(new Vector3(0, playerVelocity.y, 0) * Time.deltaTime);
    }
}
