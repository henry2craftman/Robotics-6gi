using System.Collections;
using UnityEngine;

// 목표 : 실린더 Rod를 minRange에서 maxRange로 이동시킨다.
// 속성 : 실린더 Rod의 transform, minRange, maxRange, speed
public class Cylinder : MonoBehaviour
{
    [SerializeField] Transform rod;            // private 속성인데 inspector 창에 보임.
    [SerializeField] float minRange;

    public float MinRange { get => minRange; set => value = minRange; } // Unity에서 property는 inspector 창에 보이지않음.
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

        // Coroutine 함수로 PLC에서 들어오는신호를 계속 확인. ON신호가 들어오면 이동을 1번만 실행
        StartCoroutine(MoveForwardBySignal());
        StartCoroutine(MoveBackwardBySignal());
    }

    public bool isForwardSignal; // SOL1 : PLC로 받은 X디바이스 정보를 저장
    public bool isBackwardSignal; // SOL2
    bool isFrontEnd = false; // 실린더가 앞쪽 끝에 있는 상태를 확인

    IEnumerator MoveForwardBySignal()
    {
        while (true)
        {
            yield return new WaitUntil(() => isForwardSignal && !isBackwardSignal && !isFrontEnd);

            isForwarding = true;
            isBackSWON = false;
            print("전진중");

            backwardLS.material.color = originLSColor;

            // 미리 방향을 정의
            Vector3 startPos = new Vector3(maxRange, 4.25f, 6.28f);
            Vector3 endPos = new Vector3(minRange, 4.25f, 6.28f);

            // 이동 코루틴 함수
            yield return CoMoveCylinder(startPos, endPos);

            print("이동완료");
        }
    }

    IEnumerator MoveBackwardBySignal()
    {
        while (true)
        {
            yield return new WaitUntil(() => !isForwardSignal && isBackwardSignal && isFrontEnd);

            isForwarding = false;
            isForwardSWON = false;
            print("후진중");

            forwardLS.material.color = originLSColor;

            // 미리 방향을 정의
            Vector3 startPos = new Vector3(maxRange, 4.25f, 6.28f);
            Vector3 endPos = new Vector3(minRange, 4.25f, 6.28f);

            // 이동 코루틴 함수
            yield return CoMoveCylinder(endPos, startPos);

            print("이동완료");
        }
    }

    // CylinderForward 버튼을 누르면 Cylinder 전진.
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
