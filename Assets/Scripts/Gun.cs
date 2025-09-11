using UnityEngine;

// 목표: 마우스 왼쪽버튼을 클릭하면, bulletPos에서 bullet이 생성된다.
// 속성: bulletPos, bullet
public class Gun : MonoBehaviour
{
    public Transform bulletPos;
    public Bullet bulletPrefab;
    int maxBulletNum = 10;
    public Bullet[] bullets; // 오브젝트풀(Object Pool)
    int bulletCnt = 0;
    public GameObject fireEffect;
    public float fireEffectDuration = 0.3f;
    public GameObject hitEffect;
    public bool isHit = false;
    public Vector3 hitPos = Vector3.zero;
    AudioSource audio;
    public AudioClip fireClip;
    public AudioClip reloadClip;
    public AudioClip hitClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bullets = new Bullet[maxBulletNum];

        // 10개의 Bullet을 미리 만들어놓고 오브젝트풀에 넣고 꺼놓기
        for (int i = 0; i < maxBulletNum; i++)
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.gameObject.name = "Bullet";
            bullets[i] = bullet;

            bullet.gameObject.SetActive(false);
        }

        audio = GetComponent<AudioSource>();
        audio.clip = fireClip;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0: 왼쪽, 1: 휠, 2: 오른쪽
        {
            print("Fire~!");

            if (bulletPrefab != null)
            {
                if (bulletCnt > bullets.Length - 1)
                {
                    print("총알을 모두 소진했습니다. 재장전 해주세요.");

                    return;
                }

                Bullet bullet = bullets[bulletCnt]; // 클래스(참조타입), 미리 저장해 놓은 bullets의 주소를 넣어줌
                bullet.transform.position = bulletPos.position; // 총알을 총구에 위치시킨다.
                bullet.transform.rotation = Quaternion.Euler(transform.eulerAngles.x + 90, transform.eulerAngles.y, transform.eulerAngles.z); // 총알을 총구의 회전방향으로 회전시킨다.

                bullet.gameObject.SetActive(true);

                FireEffectON();
                audio.clip = fireClip;
                audio.Play();

                Invoke("FireEffectOFF", fireEffectDuration);

                bulletCnt++;
            }
        }

        // 재장전
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            print("장전완료!");
            audio.clip = reloadClip;
            audio.Play();

            bulletCnt = 0;
        }

        HitCheck();
    }

    private void HitCheck()
    {
        if (isHit)
        {
            hitEffect.transform.position = hitPos;
            hitEffect.gameObject.SetActive(true);

            Invoke("HitEffectOff", fireEffectDuration);
        }
    }

    private void HitEffectOff()
    {
        isHit = false;

        hitEffect.gameObject.SetActive(false);
    }

    private void FireEffectON()
    {
        fireEffect.SetActive(true);
        fireEffect.transform.position = bulletPos.position;
        fireEffect.transform.rotation = transform.rotation;
    }

    private void FireEffectOFF()
    {
        fireEffect.SetActive(false);
    }


}
