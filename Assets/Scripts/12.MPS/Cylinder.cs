using System.Collections;
using UnityEngine;

// ��ǥ : �Ǹ����� Rod�� MinRange���� MaxRange�� Ư���ӵ� �̵���Ų��.
// �Ӽ� : Rod�� transform, minRange, maxRange, speed
public class Cylinder : MonoBehaviour
{
    // �����̺� �Ӽ�
    [SerializeField] Transform rod;
    [SerializeField] float minRange;
    // Unity���� ������Ƽ�� �ν�����â�� ������ ����. �׷��� �Ʒ�ó�� ó���ؾ���.
    public float MinRange { get => minRange; set => value = minRange; }
    [SerializeField] float maxRange;
    [SerializeField] float speed;

    // CylinderForward ��ư�� ������ �Ǹ����� �����Ѵ�.
    public void OnCylinderForwardEvent()
    {
        Vector3 startPos = new Vector3(minRange, 0, 0);
        Vector3 endPos = new Vector3(maxRange, 0, 0);

        StartCoroutine(CoMoveCylinder(startPos, endPos));
    }
    public void OnCylinderBackEvent()
    {
        Vector3 startPos = new Vector3(minRange, 0, 0);
        Vector3 endPos = new Vector3(maxRange, 0, 0);

        StartCoroutine(CoMoveCylinder(endPos,startPos));
    }

    IEnumerator CoMoveCylinder(Vector3 from, Vector3 to) 
    {
        while (true)
        {
            // �̵� �ڵ�
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
