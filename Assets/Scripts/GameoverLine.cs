using UnityEngine;

public class GameoverLine : MonoBehaviour
{
    AudioSource audio;
    Collider ballCollider;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ball"))
        {
            print("Game Over : �ٽý����Ϸ��� ESCŰ�� �����ּ���.");

            audio.Play();
        }
    }

}
