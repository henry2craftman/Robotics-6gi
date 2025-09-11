using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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
    Color OriginLSColor;
    [SerializeField] bool isForwardSWON = false;
    [SerializeField] bool isBackSWON = true;
    bool isForwarding = false;


    private void Start()
    {
        OriginLSColor = forwardLS.material.color;
        backwardLS.material.color = Color.green;
    }

    // CylinderForward ��ư�� ������ �Ǹ����� �����Ѵ�.
    public void OnCylinderForwardEvent()
    {
        Vector3 startPos = new Vector3(0, minRange, 0);
        Vector3 endPos = new Vector3(0, maxRange, 0);

        isForwardSWON = false;
        isForwarding = true;

        StartCoroutine(CoMoveCylinder(startPos, endPos));

        backwardLS.material.color = OriginLSColor;
    }

    IEnumerator CoMoveCylinder(Vector3 from, Vector3 to)
    {
        while (true)
        {
            Vector3 dir = to - rod.localPosition;

            float distance = dir.magnitude;

            if (distance < 0.1f)
            {

                if (isForwarding) //������ȣ�� �޾�����
                {
                    isForwardSWON = true;

                    forwardLS.material.color = Color.green;
                }
                else if(!isForwarding)//������ȣ�� �޾�����
                {
                    isBackSWON = true;

                    backwardLS.material.color = Color.green;
                }

                    isForwardSWON = !isForwardSWON;
                isBackSWON  = !isBackSWON;
                break;
            }

            rod.localPosition += dir.normalized * speed * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }

    public void OnCylinderBackwardEvent() 
    {
        Vector3 startPos = new Vector3(0, minRange, 0);
        Vector3 endPos = new Vector3(0, maxRange, 0);

        isForwardSWON = false;
        isForwarding = false;
        StartCoroutine(CoMoveCylinder(endPos, startPos));

        forwardLS.material.color = OriginLSColor;
    }
}
