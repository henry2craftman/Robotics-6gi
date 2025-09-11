using UnityEngine;

public class PointBall : MonoBehaviour
{
    public PinballManager manager;
    public int pointAmaount;
    AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ball"))
        {
            manager.totalPonint += pointAmaount;
            print($"{pointAmaount}�� ȹ���ϼ̽��ϴ�!(�� {manager.totalPonint}��)");

            audio.Play();
        }

    }
}
