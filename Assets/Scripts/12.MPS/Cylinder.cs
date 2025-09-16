using System.Collections;
using UnityEngine;

// ��ǥ : �Ǹ��� Rod�� minRange���� maxRange�� �̵���Ų��.
// �Ӽ� : �Ǹ��� Rod�� transform, minRange, maxRange, speed
public class Cylinder : MonoBehaviour
{
    [SerializeField] Transform rod;            // private �Ӽ��ε� inspector â�� ����.
    [SerializeField] float minRange;

    public float MinRange { get => minRange; set => value = minRange; } // Unity���� property�� inspector â�� ����������.
    [SerializeField] float maxRange;
    [SerializeField] float speed;

    [SerializeField] Renderer forwardLS;
    [SerializeField] Renderer backwardLS;
    Color originLSColor;

    public bool isForwardSWON = false;
    public bool isBackSWON = true;

    bool isForwarding = false;

    private void Start()
    {
        originLSColor = forwardLS.material.color;
        backwardLS.material.color = Color.green;

        // Coroutine �Լ��� PLC���� �����½�ȣ�� ��� Ȯ��. ON��ȣ�� ������ �̵��� 1���� ����
        StartCoroutine(MoveForwardBySignal());
        StartCoroutine(MoveBackwardBySignal());
    }

    public bool isForwardSignal; // SOL1 : PLC�� ���� X����̽� ������ ����
    public bool isBackwardSignal; // SOL2
    bool isFrontEnd = false; // �Ǹ����� ���� ���� �ִ� ���¸� Ȯ��

    IEnumerator MoveForwardBySignal()
    {
        while (true)
        {
            yield return new WaitUntil(() => isForwardSignal && !isBackwardSignal && !isFrontEnd);

            isForwarding = true;
            isBackSWON = false;
            print("������");

            backwardLS.material.color = originLSColor;

            // �̸� ������ ����
            Vector3 startPos = new Vector3(maxRange, 4.25f, 6.28f);
            Vector3 endPos = new Vector3(minRange, 4.25f, 6.28f);

            // �̵� �ڷ�ƾ �Լ�
            yield return CoMoveCylinder(startPos, endPos);

            print("�̵��Ϸ�");
        }
    }

    IEnumerator MoveBackwardBySignal()
    {
        while (true)
        {
            yield return new WaitUntil(() => !isForwardSignal && isBackwardSignal && isFrontEnd);

            isForwarding = false;
            isForwardSWON = false;
            print("������");

            forwardLS.material.color = originLSColor;

            // �̸� ������ ����
            Vector3 startPos = new Vector3(maxRange, 4.25f, 6.28f);
            Vector3 endPos = new Vector3(minRange, 4.25f, 6.28f);

            // �̵� �ڷ�ƾ �Լ�
            yield return CoMoveCylinder(endPos, startPos);

            print("�̵��Ϸ�");
        }
    }

    // CylinderForward ��ư�� ������ Cylinder ����.
    public void OnCylinderForwardEvent()
    {
        Vector3 startPos = new Vector3(maxRange, 4.25f, 6.28f);
        Vector3 endPos = new Vector3(minRange, 4.25f, 6.28f);

        isBackSWON = false;

        isForwarding = true;

        StartCoroutine(CoMoveCylinder(startPos, endPos));

        backwardLS.material.color = originLSColor;
    }

    public void OnCylinderBackwardEvent()
    {
        Vector3 startPos = new Vector3(maxRange, 4.25f, 6.28f);
        Vector3 endPos = new Vector3(minRange, 4.25f, 6.28f);

        isForwardSWON = false;

        isForwarding = false;

        StartCoroutine(CoMoveCylinder(endPos, startPos));

        forwardLS.material.color = originLSColor;
    }

    IEnumerator CoMoveCylinder(Vector3 from, Vector3 to)
    {
        while (true)
        {
            Vector3 dir = to - rod.localPosition;

            float distance = dir.magnitude;

            if(distance < 0.1f)
            {
                if(isForwarding)
                {
                    isForwardSWON = true;

                    forwardLS.material.color = Color.green;

                    isFrontEnd = true;
                }

                else if(!isForwarding)
                {
                    isBackSWON = true;

                    backwardLS.material.color = Color.green;
                }
                    break;
            }

            dir.Normalize();

            rod.localPosition += dir * speed * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        
    }
}
