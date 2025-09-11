using UnityEngine;

// 목표: 물체에 닿으면 사라진다.
public class Bomb : MonoBehaviour
{
    public int seconds = 3;

    // 충돌 즉시 실행되는 이벤트 함수
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Wall")
        {
            // 특수함수: 특정 시간 후 다른 함수를 발동
            Invoke("DestroyBySeconds", seconds);
        }
    }

    private void DestroyBySeconds()
    {
        print("파괴되었다!");
        Destroy(this.gameObject);
    }
}
