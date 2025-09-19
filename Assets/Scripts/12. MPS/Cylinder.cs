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
    public bool isForwardSWON = false;
    public bool isBackSWON = true;
    bool isForwarding = false; // Cylinder가 앞쪽으로 가고있을 때 true

    private void Start()
    {
        originLSColor = forwardLS.material.color;
        backwardLS.material.color = Color.green;

        // 코루틴 함수로 PLC에서 들어오는 신호를 계속확인, ON신호가 들어오면 이동을 1번만 실행
        StartCoroutine(MoveForwardBySignal());
        StartCoroutine(MoveBackwardBySignal());
    }

    public bool isForwardSignal; // SOL1 PLC로 받은 X디바이스 정보를 저장
    public bool isBackwardSignal; // SOL2 
    bool isFrontEnd = false; // 실린더가 앞쪽 끝에 있는 상태를 확인(false면 뒷끝에 있는 상태)
    IEnumerator MoveForwardBySignal()
    {
        while(true)
        {
            // 양방향 솔레노이드는 한쪽의 시그널로 실린더를 움직인다.
            yield return new WaitUntil(() =>  isForwardSignal && !isBackwardSignal && !isFrontEnd);

            isForwarding = true;
            isBackSWON = false;
            print("전진중");

            // 뒤쪽 리미트 스위치는 OFF
            backwardLS.material.color = originLSColor;

            // 미리 방향을 정의
            Vector3 startPos = new Vector3(0, minRange, 0);
            Vector3 endPos = new Vector3(0, maxRange, 0);

            // 이동 코루틴 함수
            yield return CoMoveCylinder(startPos, endPos);

            print("이동완료");
        }
    }

    IEnumerator MoveBackwardBySignal()
    {
        while (true)
        {
            // 양방향 솔레노이드는 한쪽의 시그널로 실린더를 움직인다.
            yield return new WaitUntil(() => !isForwardSignal && isBackwardSignal && isFrontEnd);

            isForwarding = false;
            isForwardSWON = false;
            print("후진중");

            // 앞쪽 리미트 스위치는 OFF
            forwardLS.material.color = originLSColor;

            // 미리 방향을 정의
            Vector3 startPos = new Vector3(0, minRange, 0);
            Vector3 endPos = new Vector3(0, maxRange, 0);

            // 이동 코루틴 함수
            yield return CoMoveCylinder(endPos, startPos);

            print("이동완료");
        }
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

                    isFrontEnd = true;
                }
                else if(!isForwarding) // 후진 신호를 받았을 때
                { 
                    isBackSWON = true;  // 도착시 Backward LimitSW ON
                    backwardLS.material.color = Color.green;

                    isFrontEnd = false;
                }

                break;
            }

            rod.localPosition += dir.normalized * speed * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }
}
