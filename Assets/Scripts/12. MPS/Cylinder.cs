using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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
    Color OriginLSColor;
    [SerializeField] bool isForwardSWON = false;
    [SerializeField] bool isBackSWON = true;
    bool isForwarding = false;


    private void Start()
    {
        OriginLSColor = forwardLS.material.color;
        backwardLS.material.color = Color.green;
    }

    // CylinderForward 버튼을 누르면 실린더가 전진한다.
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

                if (isForwarding) //전진신호를 받았을때
                {
                    isForwardSWON = true;

                    forwardLS.material.color = Color.green;
                }
                else if(!isForwarding)//후진신호를 받았을때
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
