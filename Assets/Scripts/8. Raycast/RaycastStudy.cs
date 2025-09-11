using System;
using Unity.VisualScripting;
using UnityEngine;

// ��ǥ: ���� Ŭ���� ��ü�� Ȯ���ϰ� �ʹ�.
// ��� �߰�: ���콺�� Ŭ���� ���� �� ��ü�� ���� �� �ְ�, Ŭ���߿��� ��ü�� �̵���ų �� �ִ�.
//            ���콺 Ŭ���� ������ �� ��ü�� ���� �� �ִ�.
public class RaycastStudy : MonoBehaviour
{
    public bool isGrabbing = false;
    Rigidbody rb;

    // ������� Loop �޼���
    private void FixedUpdate()
    {
        BasicRaycast();

        if (Input.GetMouseButtonDown(0))
        {
            // ��ũ�� �����̽�(���Ӻ�)�� Ŭ���ؼ� ���ϴ� ��ü�� �Ǻ��ϰ� �ʹ�.
            Vector3 mousePos = Input.mousePosition; // Ŭ���� ȭ�鿵�� ��ġ

            Ray ray = Camera.main.ScreenPointToRay(mousePos); // ī�޶��� ��ġ���� ��ũ������Ʈ�� ���ϴ� ����

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

            // ��ũ�� �����̽�(���Ӻ�)�� Ŭ���ؼ� ���ϴ� ��ü�� �Ǻ��ϰ� �ʹ�.
            Vector3 mousePos = Input.mousePosition; // Ŭ���� ȭ�鿵�� ��ġ

            Ray ray = Camera.main.ScreenPointToRay(mousePos); // ī�޶��� ��ġ���� ��ũ������Ʈ�� ���ϴ� ����

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 5)) // ���̸� �߻��Ѵ�. 5m��ŭ
            {
                Debug.DrawRay(transform.position, ray.direction * 5, Color.green); // ���̸� �׸���.

                if(hitInfo.collider.gameObject.CompareTag("GrabableObject"))
                {
                    Vector3 newPos = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.collider.transform.position.z);
                    
                    // �� ĳ������ �̵���

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

            // ��ũ�� �����̽�(���Ӻ�)�� Ŭ���ؼ� ���ϴ� ��ü�� �Ǻ��ϰ� �ʹ�.
            Vector3 mousePos = Input.mousePosition; // Ŭ���� ȭ�鿵�� ��ġ

            Ray ray = Camera.main.ScreenPointToRay(mousePos); // ī�޶��� ��ġ���� ��ũ������Ʈ�� ���ϴ� ����

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 5))
            {
                Debug.DrawRay(transform.position, ray.direction * 5, Color.green);

                if (hitInfo.collider.gameObject.CompareTag("GrabableObject"))
                {
                    print(hitInfo.point);
                    Vector3 newPos = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.collider.transform.position.z);
                    // �� ĳ������ �̵���
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

    // �����Ϳ� ����� �׸� �� ���
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
