using UnityEngine;

// 목표: 키입력을 받아서 Sphere에 힘을 주고싶다.
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
            Vector3 dir = transform.forward + transform.up; // 대각선방향 = 앞방향 + 위방향

            rb.AddForce(dir * power, ForceMode.Impulse); // 순간적인 힘을 가한다
        }

        float turn = Input.GetAxis("Horizontal");
        rb.AddTorque(transform.right * torque * turn);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name + "와 충돌시작!");
    }

    private void OnCollisionStay(Collision collision)
    {
        print(collision.gameObject.name + "와 접촉중");
    }

    private void OnCollisionExit(Collision collision)
    {
        print(collision.gameObject.name + "와 충돌해지");
    }
}
