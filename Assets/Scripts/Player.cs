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
    public float jumpHeight = 3f;  // 점프 높이 (이것으로 점프 힘을 계산하는 것이 더 직관적)
    public Transform nPC;
    private Vector3 playerVelocity; // 플레이어의 현재 속도를 관리할 변수
    private bool isGrounded;

    // LifeCycle 함수
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("Player가 생성됨!");

        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move();
        //MoveByJoypad();

        RotateBody();

        isGrounded = controller.isGrounded;

        // 땅에 있을 때 Y축 속도 초기화 (중력 누적 방지)
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // 땅에 붙어있도록 약간의 음수 값 유지 (옵션)
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
        float mouseX = Input.GetAxis("Mouse X"); // -1 ~ 1 사이의 값

        //print($"Mouse Input: {mouseX}, {mouseY}");

        float dy = mouseX * rotSpeed * Time.deltaTime;

        transform.eulerAngles += new Vector3(0, dy, 0);
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //transform.position = transform.position + Vector3.left; // Vector3.left 월드좌표(절대좌표)의 왼쪽 방향
            transform.position -= transform.right * moveSpeed * Time.deltaTime; // 로컬좌표의 왼쪽 방향
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            //transform.position = transform.position + Vector3.right; // Vector3.right 월드좌표의 오른쪽 방향 방향
            transform.position += transform.right * moveSpeed * Time.deltaTime; // 로컬좌표의 오른쪽 방향
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

    // 가감속 추가
    private void MoveByJoypad()
    {
        float horizontal = Input.GetAxis("Horizontal"); // 키보드 왼쪽, 오른쪽 -1 ~ 1로 표현
        float vertical = Input.GetAxis("Vertical");     // 키보드 방향키 위, 아래

        Vector3 direction = (transform.forward * vertical) + (transform.right * horizontal);

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void MoveByCharacterController()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 이동 방향 계산 (앞/뒤, 좌/우)
        Vector3 moveDirection = (transform.forward * vertical) + (transform.right * horizontal);

        // 이동 (Y축 속도 제외)
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
    private void ApplyJumpAndGravity()
    {
        // 점프 입력 처리
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            // 점프 높이로부터 초기 점프 속도 계산 공식:
            // v = sqrt(h * -2 * g)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 중력 적용
        playerVelocity.y += gravity * Time.deltaTime;

        // 최종 Y축 속도를 CharacterController에 적용
        controller.Move(new Vector3(0, playerVelocity.y, 0) * Time.deltaTime);
    }
}
