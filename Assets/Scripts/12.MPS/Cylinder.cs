using System.Collections;
using UnityEngine;

// 목표 : 실린더 Rod를 minRange에서 maxRange로 이동시킨다.
// 속성 : 실린더 Rod의 transform, minRange, maxRange, speed
public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] Transform rod;            // private 속성인데 inspector 창에 보임.
    [SerializeField] float minRange;

    public float MinRange { get => minRange; set => value = minRange; } // Unity에서 property는 inspector 창에 보이지않음.
    [SerializeField] float maxRange;
    [SerializeField] float speed;

    // CylinderForward 버튼을 누르면 Cylinder 전진.
    public void OnCylinderForwardEvent()
    {
        Vector3 startPos = new Vector3(maxRange, 4.25f, 6.28215f);
        Vector3 endPos = new Vector3(minRange, 4.25f, 6.28215f);
        StartCoroutine(CoMoveCylinder(startPos, endPos));
    }

    IEnumerator CoMoveCylinder(Vector3 from, Vector3 to)
    {
        while (true)
        {
            Vector3 dir = to - rod.localPosition;

            float distance = dir.magnitude;

            if(distance < 0.1f)
            {
                break;
            }

            dir.Normalize();

            rod.localPosition += dir * speed * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        
    }
}
