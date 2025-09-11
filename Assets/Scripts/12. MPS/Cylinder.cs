using System.Collections;
using UnityEngine;

// ��ǥ: �Ǹ����� Rod�� minRange���� maxRange�� Ư�� �ӵ��� �̵���Ų��.
// �Ӽ�: Rod�� transform, minRange, maxRange, �ӵ�
public class Cylinder : MonoBehaviour
{
    [SerializeField] Transform rod;
    [SerializeField] float minRange;
    // Unity���� ������Ƽ�� �ν�����â�� ������ ����.
    public float MinRange { get => minRange; set => value = minRange; }
    [SerializeField] float maxRange;
    [SerializeField] float speed;
    [SerializeField] Renderer forwardLS;
    [SerializeField] Renderer backwardLS;
    Color originLSColor;
    [SerializeField] bool isForwardSWON = false;
    [SerializeField] bool isBackSWON = true;
    bool isForwarding = false; // Cylinder�� �������� �������� �� true

    private void Start()
    {
        originLSColor = forwardLS.material.color;
        backwardLS.material.color = Color.green;
    }

    // CylinderForward ��ư�� ������ �Ǹ����� �����Ѵ�.
    public void OnCylinderForwardEvent()
    {
        Vector3 startPos = new Vector3(0, minRange, 0);
        Vector3 endPos = new Vector3(0, maxRange, 0);

        isBackSWON = false; 

        isForwarding = true; // ������ ���ϱ� ������ ��(������ �����̴� ��)

        StartCoroutine(CoMoveCylinder(startPos, endPos));

        backwardLS.material.color = originLSColor;
    }

    public void OnCylinderBackwardEvent()
    {
        Vector3 startPos = new Vector3(0, minRange, 0);
        Vector3 endPos = new Vector3(0, maxRange, 0);

        isForwardSWON = false;

        isForwarding = false; // �ڷ� ���ϱ� ������ ��(�ڷ� �����̴� ��)

        StartCoroutine(CoMoveCylinder(endPos, startPos));

        forwardLS.material.color = originLSColor;
    }

    IEnumerator CoMoveCylinder(Vector3 from, Vector3 to)
    {
        while (true)
        {
            Vector3 dir = to - rod.localPosition;

            float distance = dir.magnitude;

            if (distance < 0.1f)
            {
                if (isForwarding) // ���� ��ȣ�� �޾��� ��
                {
                    isForwardSWON = true; // ������ Forward LimitSW ON
                    forwardLS.material.color = Color.green;
                }
                else if(!isForwarding) // ���� ��ȣ�� �޾��� ��
                { 
                    isBackSWON = true;  // ������ Backward LimitSW ON
                    backwardLS.material.color = Color.green;
                }

                break;
            }

            rod.localPosition += dir.normalized * speed * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }
}
