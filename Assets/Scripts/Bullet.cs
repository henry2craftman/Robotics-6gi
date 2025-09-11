using UnityEngine;

// ��ǥ: �¾�ڸ��� Ư�� �������� Ư���ӵ��� �̵��Ѵ�.
// �Ӽ�: �ӵ�, ����
public class Bullet : MonoBehaviour
{
    public float speed = 10;
    public Vector3 dir;
    Rigidbody rb;
    Gun gun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gun = FindAnyObjectByType<Gun>();
    }

    // Update is called once per frame
    void Update()
    {
        dir = transform.up;

        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        gun.isHit = true;
        gun.hitPos = transform.position;

        rb.angularVelocity = Vector3.zero; // ���ӵ�(����� �ӵ��� ����)
        rb.linearVelocity = Vector3.zero;  // �����ӵ�(������� �ӵ��� ����)

        gameObject.SetActive(false);
    }
}
