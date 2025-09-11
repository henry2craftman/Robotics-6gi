using System.Collections;
using UnityEngine;

// 목표: 실린더의 Rod를 minRange에서 maxRange로 특정 속도로 이동시킨다.
// 속성: Rod의 transform, minRange, maxRange, 속도
public class Cylinder : MonoBehaviour
{
    [SerializeField] Transform rod;
    [SerializeField] float minRange;
    // Unity에서 프로퍼티는 인스펙터창에 보이지 않음.
    public float MinRange { get => minRange; set => value = minRange; }
    [SerializeField] float maxRange;
    [SerializeField] float speed;

    // CylinderForward 버튼을 누르면 실린더가 전진한다.
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
