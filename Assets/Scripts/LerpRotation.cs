using UnityEngine;

// ��ǥ: Quaternion�� ��� �� Lerp�� ����Ͽ� �� �ڽ��� ȸ����Ų��.
// �Ӽ�: ���� ����, �� ����, �ð�����
public class LerpRotation : MonoBehaviour
{
    public float time;          // �󸶵��� ȸ������
    public float elapsedTime;   // ����ð�
    public float startAngle;
    public float endAngle;
    Quaternion startQ;          // ȸ�� �� ����
    Quaternion endQ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startQ = Quaternion.AngleAxis(startAngle, transform.forward);
        endQ   = Quaternion.AngleAxis(endAngle, transform.forward);

        transform.rotation = endQ;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime; // �� �������� �ð��� ������

        if( elapsedTime > time)
        {
            elapsedTime = 0;
        }

        // ������ ���� ȸ���� ���� �������� ��ȯ
        Quaternion q = Quaternion.Lerp(startQ, endQ, elapsedTime / time);

        transform.rotation = q; // q�� ȸ���� ����
    }
}
