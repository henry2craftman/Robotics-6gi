using UnityEngine;
using System.Threading;


// 목표 : Thread를 사용한 비동기 프로그램 예제
// 키 입력을 받았을 때, 렉 유발 함수를 만든다. 렉과 상관없이 프로그램이 작동한다.
public class ThreadManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //1. 문제상황
        if(Input.GetKeyDown(KeyCode.Alpha1)) // keyboard 1번 key 사용
        {
            Debug.Log("렉 유발함수 호출");
            LaggyFunction();
            Debug.Log("함수 종료.");
        }

        //2. 해결책
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("렉 유발함수 호출");
            Thread thread = new Thread(LaggyFunction);
            thread.Start();

            Debug.Log("함수 종료.");
        }

        Debug.Log("게임실행중");
    }

    // 렉 발생함수
    void LaggyFunction()
    {
        Debug.Log("PLC에 데이터를 요청합니다.");

        // 실제로는 계산이 오래걸리는 알고리즘이 들어감
        Thread.Sleep(5000);

        Debug.Log("PLC로부터 데이터를 받음.");
    }
}
