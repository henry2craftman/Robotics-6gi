using UnityEngine;

// ��ǥ: ��ü�� ������ �������.
public class Bomb : MonoBehaviour
{
    public int seconds = 3;

    // �浹 ��� ����Ǵ� �̺�Ʈ �Լ�
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Wall")
        {
            // Ư���Լ�: Ư�� �ð� �� �ٸ� �Լ��� �ߵ�
            Invoke("DestroyBySeconds", seconds);
        }
    }

    private void DestroyBySeconds()
    {
        print("�ı��Ǿ���!");
        Destroy(this.gameObject);
    }
}
