using UnityEngine;

// 목표: 태어나자마자 특정 방향으로 특정속도로 이동한다.
// 속성: 속도, 방향
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

        rb.angularVelocity = Vector3.zero; // 각속도(원운동의 속도와 방향)
        rb.linearVelocity = Vector3.zero;  // 선형속도(직선운동의 속도와 방향)

        gameObject.SetActive(false);
    }
}
