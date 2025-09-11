using System;
using Unity.VisualScripting;
using UnityEngine;

// 목표: 내가 클릭한 물체를 확인하고 싶다.
// 기능 추가: 마우스로 클릭을 했을 때 물체를 잡을 수 있고, 클릭중에는 물체를 이동시킬 수 있다.
//            마우스 클릭을 놓았을 때 물체를 놓을 수 있다.
public class RaycastStudy : MonoBehaviour
{
    public bool isGrabbing = false;
    Rigidbody rb;

    // 물리계산 Loop 메서드
    private void FixedUpdate()
    {
        BasicRaycast();

        if (Input.GetMouseButtonDown(0))
        {
            // 스크린 스페이스(게임뷰)를 클릭해서 원하는 물체를 판별하고 싶다.
            Vector3 mousePos = Input.mousePosition; // 클릭한 화면영역 위치

            Ray ray = Camera.main.ScreenPointToRay(mousePos); // 카메라의 위치에서 스크린포인트로 향하는 레이

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 5))
            {
                Debug.DrawRay(transform.position, ray.direction * 5, Color.red);

                if(hitInfo.collider.gameObject.CompareTag("GrabableObject"))
                {
                    print(hitInfo.collider.gameObject.tag);
                    rb = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                    rb.useGravity = false;

                    isGrabbing = true;
                }
            }
        }

        if(Input.GetMouseButton(0))
        {
            if (!isGrabbing)
                return;

            // 스크린 스페이스(게임뷰)를 클릭해서 원하는 물체를 판별하고 싶다.
            Vector3 mousePos = Input.mousePosition; // 클릭한 화면영역 위치

            Ray ray = Camera.main.ScreenPointToRay(mousePos); // 카메라의 위치에서 스크린포인트로 향하는 레이

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 5)) // 레이를 발사한다. 5m만큼
            {
                Debug.DrawRay(transform.position, ray.direction * 5, Color.green); // 레이를 그린다.

                if(hitInfo.collider.gameObject.CompareTag("GrabableObject"))
                {
                    Vector3 newPos = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.collider.transform.position.z);
                    
                    // 내 캐릭터의 이동양

                    hitInfo.collider.transform.position = newPos;
                }
            }
            else
            {
                if(rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.useGravity = true;
                }

                isGrabbing = false;
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            if (!isGrabbing) 
                return;

            // 스크린 스페이스(게임뷰)를 클릭해서 원하는 물체를 판별하고 싶다.
            Vector3 mousePos = Input.mousePosition; // 클릭한 화면영역 위치

            Ray ray = Camera.main.ScreenPointToRay(mousePos); // 카메라의 위치에서 스크린포인트로 향하는 레이

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 5))
            {
                Debug.DrawRay(transform.position, ray.direction * 5, Color.green);

                if (hitInfo.collider.gameObject.CompareTag("GrabableObject"))
                {
                    print(hitInfo.point);
                    Vector3 newPos = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.collider.transform.position.z);
                    // 내 캐릭터의 이동양
                    hitInfo.collider.transform.position = newPos;

                    rb = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.useGravity = true;

                    isGrabbing = false;
                }
            }
        }
    }

    // 에디터에 기즈모를 그릴 때 사용
    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere()
    }

    private void BasicRaycast()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 20))
        {
            print(hitInfo.collider.gameObject.name);
            Debug.DrawRay(transform.position, transform.forward * 20, Color.red);
        }
    }
}
