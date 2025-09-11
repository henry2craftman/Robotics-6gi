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
    [SerializeField] Renderer forwardLS;
    [SerializeField] Renderer backwardLS;
    Color originLSColor;
    [SerializeField] bool isForwardSWON = false;
    [SerializeField] bool isBackSWON = true;
    bool isForwarding = false; // Cylinder가 앞쪽으로 가고있을 때 true

    private void Start()
    {
        originLSColor = forwardLS.material.color;
        backwardLS.material.color = Color.green;
    }

    // CylinderForward 버튼을 누르면 실린더가 전진한다.
    public void OnCylinderForwardEvent()
    {
        Vector3 startPos = new Vector3(0, minRange, 0);
        Vector3 endPos = new Vector3(0, maxRange, 0);

        isBackSWON = false; 

        isForwarding = true; // 앞으로 향하기 시작할 때(앞으로 움직이는 중)

        StartCoroutine(CoMoveCylinder(startPos, endPos));

        backwardLS.material.color = originLSColor;
    }

    public void OnCylinderBackwardEvent()
    {
        Vector3 startPos = new Vector3(0, minRange, 0);
        Vector3 endPos = new Vector3(0, maxRange, 0);

        isForwardSWON = false;

        isForwarding = false; // 뒤로 향하기 시작할 때(뒤로 움직이는 중)

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
                if (isForwarding) // 전진 신호를 받았을 때
                {
                    isForwardSWON = true; // 도착시 Forward LimitSW ON
                    forwardLS.material.color = Color.green;
                }
                else if(!isForwarding) // 후진 신호를 받았을 때
                { 
                    isBackSWON = true;  // 도착시 Backward LimitSW ON
                    backwardLS.material.color = Color.green;
                }

                break;
            }

            rod.localPosition += dir.normalized * speed * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }
}
