using UnityEngine;

// ��ǥ: Ű�Է��� �޾Ƽ� Sphere�� ���� �ְ�ʹ�.
public class CollisionDetection : MonoBehaviour
{
    Rigidbody rb;
    public float power = 5;
    public float torque = 5;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 dir = transform.forward + transform.up; // �밢������ = �չ��� + ������

            rb.AddForce(dir * power, ForceMode.Impulse); // �������� ���� ���Ѵ�
        }

        float turn = Input.GetAxis("Horizontal");
        rb.AddTorque(transform.right * torque * turn);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name + "�� �浹����!");
    }

    private void OnCollisionStay(Collision collision)
    {
        print(collision.gameObject.name + "�� ������");
    }

    private void OnCollisionExit(Collision collision)
    {
        print(collision.gameObject.name + "�� �浹����");
    }
}
