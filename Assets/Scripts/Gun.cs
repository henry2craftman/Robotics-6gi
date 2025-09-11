using UnityEngine;

// ��ǥ: ���콺 ���ʹ�ư�� Ŭ���ϸ�, bulletPos���� bullet�� �����ȴ�.
// �Ӽ�: bulletPos, bullet
public class Gun : MonoBehaviour
{
    public Transform bulletPos;
    public Bullet bulletPrefab;
    int maxBulletNum = 10;
    public Bullet[] bullets; // ������ƮǮ(Object Pool)
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

        // 10���� Bullet�� �̸� �������� ������ƮǮ�� �ְ� ������
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
        if (Input.GetMouseButtonDown(0)) // 0: ����, 1: ��, 2: ������
        {
            print("Fire~!");

            if (bulletPrefab != null)
            {
                if (bulletCnt > bullets.Length - 1)
                {
                    print("�Ѿ��� ��� �����߽��ϴ�. ������ ���ּ���.");

                    return;
                }

                Bullet bullet = bullets[bulletCnt]; // Ŭ����(����Ÿ��), �̸� ������ ���� bullets�� �ּҸ� �־���
                bullet.transform.position = bulletPos.position; // �Ѿ��� �ѱ��� ��ġ��Ų��.
                bullet.transform.rotation = Quaternion.Euler(transform.eulerAngles.x + 90, transform.eulerAngles.y, transform.eulerAngles.z); // �Ѿ��� �ѱ��� ȸ���������� ȸ����Ų��.

                bullet.gameObject.SetActive(true);

                FireEffectON();
                audio.clip = fireClip;
                audio.Play();

                Invoke("FireEffectOFF", fireEffectDuration);

                bulletCnt++;
            }
        }

        // ������
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            print("�����Ϸ�!");
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
