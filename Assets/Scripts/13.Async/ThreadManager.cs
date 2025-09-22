using UnityEngine;

using System.Threading;

// 목표 : Thread를 사용한 비동기 프로그램 예제
// 키 입력을 받았을 때, 렉 유발함수를 만든다. 렉과 상관없이 프로그램이 작동한다.
public class ThreadManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // 1. 문제 상황
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("렉 유발함수 호출 시작");
            LaggyFunction();
            Debug.Log("함수 실행 종료");
        }
        // 2. 해결책 - thread를 사용함.
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("렉 유발함수 호출 시작");
            Thread thread = new Thread(LaggyFunction);
            thread.Start();

            Debug.Log("함수 실행 종료");
        }

        Debug.Log("게임실행중");
                                                                                                                                                                                                                        }
    


    // 렉을 발생시키는 함수
    void LaggyFunction()
    {
        Debug.Log("PLC에 데이터 요청을 시작합니다.");

        Thread.Sleep(5000); // 5초동안 현재 스레드를 Block

        Debug.Log("PLC로부터 데이터를 받았음.");

    }
}
