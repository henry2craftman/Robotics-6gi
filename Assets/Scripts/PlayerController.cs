using UnityEngine;

/// <summary>
/// 플레이어의 입력을 받아 캐릭터를 제어하는 스크립트.
/// isLocalPlayer 플래그가 true일 때만 입력을 받습니다.
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 이 캐릭터가 로컬 플레이어인지(직접 조종해야 하는지) 여부.
    /// ClientAsync 스크립트가 자신의 캐릭터에 대해 true로 설정해 줍니다.
    /// </summary>
    public bool isLocalPlayer = false;

    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 180.0f;

    void Update()
    {
        // 이 캐릭터가 로컬 플레이어가 아니면 입력을 처리하지 않음
        if (!isLocalPlayer)
        {
            return;
        }

        // --- 입력 처리 ---
        // 상하(W, S) 입력
        float verticalInput = Input.GetAxis("Vertical");
        // 좌우(A, D) 입력
        float horizontalInput = Input.GetAxis("Horizontal");

        // --- 이동 및 회전 처리 ---
        // 이동: W/S 키에 따라 앞/뒤로 이동
        transform.Translate(Vector3.forward * verticalInput * moveSpeed * Time.deltaTime);

        // 회전: A/D 키에 따라 좌/우로 회전
        transform.Rotate(Vector3.up * horizontalInput * rotationSpeed * Time.deltaTime);
    }
}
