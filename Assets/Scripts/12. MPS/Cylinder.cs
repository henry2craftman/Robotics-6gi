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

    // CylinderForward ��ư�� ������ �Ǹ����� �����Ѵ�.
    public void OnCylinderForwardEvent()
    {
        Vector3 startPos = new Vector3(0, minRange, 0);
        Vector3 endPos = new Vector3(0, maxRange, 0);

        StartCoroutine(CoMoveCylinder(startPos, endPos));
    }

    IEnumerator CoMoveCylinder(Vector3 from, Vector3 to)
    {
        while (true)
        {
            Vector3 dir = to - rod.localPosition;

            float distance = dir.magnitude;

            if (distance < 0.1f)
            {
                break;
            }

            rod.localPosition += dir.normalized * speed * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }
}
