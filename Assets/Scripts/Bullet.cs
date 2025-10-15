using UnityEngine;

// 목표: 태어나자마자 특정 방향으로 특정속도로 이동한다.
// 속성: 속도, 방향
public class Bullet : MonoBehaviour
{
    public float speed = 10;
    public Vector3 dir;
    public string ownerId; // 총알을 발사한 클라이언트의 ID
    Rigidbody rb;
    Gun gun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Gun을 찾는 로직은 Gun이 여러 개일 경우 문제가 될 수 있으므로,
        // 다른 방식으로 Gun에 대한 참조를 전달하는 것이 좋습니다. (예: 발사 시점에 설정)
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
        if (gun != null)
        {
            gun.isHit = true;
            gun.hitPos = transform.position;
        }

        rb.angularVelocity = Vector3.zero; // 회전속도(회전운동의 속도를 표현)
        rb.linearVelocity = Vector3.zero;  // 선형속도(병진운동의 속도를 표현)

        // gameObject.SetActive(false);
        // 오브젝트 풀 매니저에 총알을 반납합니다.
        if (BulletPoolManager.Instance != null)
        {
            BulletPoolManager.Instance.ReturnBullet(ownerId, this);
        }
        else
        {
            // 폴백: 풀 매니저가 없으면 그냥 비활성화
            gameObject.SetActive(false);
        }
    }
}
