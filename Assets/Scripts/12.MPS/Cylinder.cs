using System.Collections;
using UnityEngine;

// 목표 : 실린더의 Rod를 MinRange에서 MaxRange로 특정속도 이동시킨다.
// 속성 : Rod의 transform, minRange, maxRange, speed
public class Cylinder : MonoBehaviour
{
    
    // 프라이빗 속성
    [SerializeField] Transform rod;
    [SerializeField] float minRange;
    // Unity에서 프로퍼티는 인스펙터창에 보이지 않음. 그래서 아래처럼 처리해야함.
    public float MinRange { get => minRange; set => value = minRange; }
    [SerializeField] float maxRange;
    [SerializeField] float speed;
    [SerializeField] Renderer forwardLS;
    [SerializeField] Renderer backwardLS;
    Color originLSColor;
    public bool isForwardSWON = false;
    public bool isBackwardSWON = true;
    bool isForwarding = false;

    private void Start()
    {
        originLSColor = forwardLS.material.color;
        backwardLS.material.color = Color.green;

        // 코루틴 함수로 PLC에서 들어오는 신호를 계속 확인, ON신호가 들어오면 이동을 1번만 실행
        StartCoroutine(MoveForwardBySignal());
        StartCoroutine(MoveBackwardBySignal());

    }

    public bool isForwardSignal; // SOL1, PLC로 받은 X디바이스 정보를 저장
    public bool isBackwardSignal; // SOL2 
    bool isFrontEnd = false; // 실린더 앞쪽 끝에 있는 상태를 확인



    IEnumerator MoveForwardBySignal()
    {
        while (true)
        {
            yield return new WaitUntil(() => isForwardSignal && !isBackwardSignal&& !isFrontEnd);

            isForwarding = true;
            isBackwardSWON = false;

            print("전진중");
            //뒤쪽 리밋 스위치 꺼짐
            backwardLS.material.color = originLSColor;

            // 미리 방향 정의
            Vector3 startPos = new Vector3(minRange, 0, 0);
            Vector3 endPos = new Vector3(maxRange, 0, 0);

            // 이동 코루틴 함수 호출
            yield return CoMoveCylinder(startPos, endPos);

            print("이동완료");

        }
    }
    IEnumerator MoveBackwardBySignal()
    {
        while (true)
        {
            yield return new WaitUntil(() => isBackwardSignal && !isForwardSignal && isFrontEnd);

            isForwarding = false;
            isForwardSWON = false;

            print("후진중");
            //앞쪽 리밋 스위치 꺼짐
            forwardLS.material.color = originLSColor;

            // 미리 방향 정의
            Vector3 startPos = new Vector3(minRange, 0, 0);
            Vector3 endPos = new Vector3(maxRange, 0, 0);

            // 이동 코루틴 함수 호출
            yield return CoMoveCylinder(endPos, startPos);

            print("이동완료");

        }
    }

    // CylinderForward 버튼을 누르면 실린더가 전진한다.
    public void OnCylinderForwardEvent()
    {
        Vector3 startPos = new Vector3(minRange, 0, 0);
        Vector3 endPos = new Vector3(maxRange, 0, 0);
        isBackwardSWON = false;
        isForwarding = true; // 앞으로 향하기 시작할 때(앞으로 움직이는 중)
        StartCoroutine(CoMoveCylinder(startPos, endPos));
        backwardLS.material.color = originLSColor;
    }
    public void OnCylinderBackEvent()
    {
        Vector3 startPos = new Vector3(minRange, 0, 0);
        Vector3 endPos = new Vector3(maxRange, 0, 0);
        isForwardSWON = false;
        isForwarding = false; // 뒤로 향하기 시작할 때(뒤로 움직이는 중)
        StartCoroutine(CoMoveCylinder(endPos,startPos));
        forwardLS.material.color = originLSColor;

    }

    IEnumerator CoMoveCylinder(Vector3 from, Vector3 to) 
    {
        while (true)
        {
            // 이동 코드
            Vector3 dir = to - rod.localPosition;

            float distance = dir.magnitude;
            if (distance < 0.1f)
            {
                if (isForwarding)
                {
                    isForwardSWON = true;
                    forwardLS.material.color = Color.green;

                    isFrontEnd = true; // 앞끝단으로 나간 상태
                }
                else if (!isForwarding)
                {
                    isBackwardSWON = true;
                    backwardLS.material.color = Color.green;

                    isFrontEnd = false; // 뒤끝단으로 나간 상태
                }
                break;
            }
            rod.localPosition += dir.normalized * speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        

    }
    
}
