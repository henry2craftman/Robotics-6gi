using System.Collections;
using UnityEngine;

public class HandleController : MonoBehaviour
{
    public KeyCode activateKey = KeyCode.LeftShift;
    public float rotationSpeed = 500f; // �ڵ��� �ö󰡰� �������� �ӵ� (��/��)

    public float targetAngle = 45f; // Ȱ��ȭ�Ǿ��� ���� ��ǥ ����
    public float originAngle = 0f;  // ��Ȱ��ȭ�Ǿ��� ���� ��ǥ ���� (���� �ʱ� ��ġ)

    public float baseHitForce = 25f; // ���� ���� �⺻ �� (��ݷ�)

    private Quaternion initialLocalRotation; // �ڵ��� �θ� ���� �ʱ� ȸ��
    private Quaternion targetRotation;       // ���� ��ǥ ȸ��
    public bool isActivating = false;       // �ڵ��� Ȱ��ȭ(�ö󰡴�) ������
    Vector3 normal;
    AudioSource audio;

    void Start()
    {
        initialLocalRotation = transform.localRotation; // �θ� �������� �� �ʱ� ȸ�� ����
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

        // ��ǥ ȸ�� ����
        // initialLocalRotation�� Ȱ��ȭ/��Ȱ��ȭ ������ ���Ͽ� ���� ��ǥ ȸ���� ����ϴ�.
        if (isActivating)
        {
            targetRotation = initialLocalRotation * Quaternion.Euler(0, 0, targetAngle);
        }
        else
        {
            targetRotation = initialLocalRotation * Quaternion.Euler(0, 0, originAngle);
        }

        // ���� ȸ���� ��ǥ ȸ������ ����
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� "Ball" �±׸� ������ �ְ�, �ڵ��� Ȱ��ȭ(�ö󰡴�) ���� ���̶��
        if (collision.gameObject.CompareTag("ball") && isActivating)
        {
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                print(collision.contacts.Length);
                // �浹 ������ ���� ���͸� �⺻ �������� ���
                normal = collision.contacts[0].normal;

                // ���� ���Ϳ� �ڵ��� �о�� ������ ȥ���Ͽ� ���� Ÿ�� ���� ����
                // flipperPushDirection�� ����ġ�� �� �־� �ڵ��� �о�� ������ ��ȭ
                Vector3 finalHitDirection = (normal * 2f).normalized; // 2f�� ����ġ

                // ���� �� ���ϱ�
                ballRb.AddForce(finalHitDirection * baseHitForce, ForceMode.Impulse);
            }
        }
    }


    //private void OnCollisionExit(Collision collision)
    //{
    //    print("�����");

    //    // �浹�� ������Ʈ�� "Ball" �±׸� ������ �ְ�, �ڵ��� Ȱ��ȭ(�ö󰡴�) ���� ���̶��
    //    if (collision.gameObject.CompareTag("ball") && isActivating)
    //    {
    //        Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
    //        if (ballRb != null)
    //        {
    //            // ���� ���Ϳ� �ڵ��� �о�� ������ ȥ���Ͽ� ���� Ÿ�� ���� ����
    //            // flipperPushDirection�� ����ġ�� �� �־� �ڵ��� �о�� ������ ��ȭ
    //            Vector3 finalHitDirection = (normal * 2f).normalized; // 2f�� ����ġ

    //            // ���� �� ���ϱ�
    //            ballRb.AddForce(-finalHitDirection * baseHitForce, ForceMode.Impulse);
    //        }
    //    }
    //}
}
