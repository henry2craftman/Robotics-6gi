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
            print("Game Over : 다시시작하려면 ESC키를 눌러주세요.");

            audio.Play();
        }
    }

}
