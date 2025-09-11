using System.Collections;
using UnityEngine;

public class HandleController : MonoBehaviour
{
    public KeyCode activateKey = KeyCode.LeftShift;
    public float rotationSpeed = 500f; // 핸들이 올라가고 내려가는 속도 (도/초)

    public float targetAngle = 45f; // 활성화되었을 때의 목표 각도
    public float originAngle = 0f;  // 비활성화되었을 때의 목표 각도 (보통 초기 위치)

    public float baseHitForce = 25f; // 공에 가할 기본 힘 (충격력)

    private Quaternion initialLocalRotation; // 핸들의 부모 기준 초기 회전
    private Quaternion targetRotation;       // 현재 목표 회전
    public bool isActivating = false;       // 핸들이 활성화(올라가는) 중인지
    Vector3 normal;
    AudioSource audio;

    void Start()
    {
        initialLocalRotation = transform.localRotation; // 부모를 기준으로 한 초기 회전 저장
        targetRotation = initialLocalRotation;

        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(activateKey))
        {
            isActivating = true;

            audio.Play();
        }
        else if (Input.GetKeyUp(activateKey))
        {
            isActivating = false;
        }

        // 목표 회전 설정
        // initialLocalRotation에 활성화/비활성화 각도를 더하여 최종 목표 회전을 만듭니다.
        if (isActivating)
        {
            targetRotation = initialLocalRotation * Quaternion.Euler(0, 0, targetAngle);
        }
        else
        {
            targetRotation = initialLocalRotation * Quaternion.Euler(0, 0, originAngle);
        }

        // 현재 회전을 목표 회전으로 보간
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 "Ball" 태그를 가지고 있고, 핸들이 활성화(올라가는) 동작 중이라면
        if (collision.gameObject.CompareTag("ball") && isActivating)
        {
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                print(collision.contacts.Length);
                // 충돌 지점의 법선 벡터를 기본 방향으로 사용
                normal = collision.contacts[0].normal;

                // 법선 벡터와 핸들의 밀어내는 방향을 혼합하여 최종 타격 방향 생성
                // flipperPushDirection의 가중치를 더 주어 핸들이 밀어내는 느낌을 강화
                Vector3 finalHitDirection = (normal * 2f).normalized; // 2f는 가중치

                // 공에 힘 가하기
                ballRb.AddForce(finalHitDirection * baseHitForce, ForceMode.Impulse);
            }
        }
    }


    //private void OnCollisionExit(Collision collision)
    //{
    //    print("통과됨");

    //    // 충돌한 오브젝트가 "Ball" 태그를 가지고 있고, 핸들이 활성화(올라가는) 동작 중이라면
    //    if (collision.gameObject.CompareTag("ball") && isActivating)
    //    {
    //        Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
    //        if (ballRb != null)
    //        {
    //            // 법선 벡터와 핸들의 밀어내는 방향을 혼합하여 최종 타격 방향 생성
    //            // flipperPushDirection의 가중치를 더 주어 핸들이 밀어내는 느낌을 강화
    //            Vector3 finalHitDirection = (normal * 2f).normalized; // 2f는 가중치

    //            // 공에 힘 가하기
    //            ballRb.AddForce(-finalHitDirection * baseHitForce, ForceMode.Impulse);
    //        }
    //    }
    //}
}
