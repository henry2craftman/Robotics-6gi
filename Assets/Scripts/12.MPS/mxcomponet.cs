using UnityEngine;
using ActUtlType64Lib;
using System;


// 목표 : UI(연결, 연결해제) 버튼을 누르면 실시간으로 PLC에 데이터를 요청하고 쓴다.
// 속성 : mxComponent 객체변수, 요청하는 기능, 쓰기 기능
// PLC에 요청할 값들 : SOL, LS(limit sensor), SENSOR, CONVEYOR, TOWERLAMP, Loader
// X(input 신호) : LS신호(X0~7), Sensor 신호(X8, x9,x0a(Loader))
// Y(output 신호) : Sol 신호(Y0~Y7), Conveyor 신호(Y8(on/off), Y9(CW), Y0A(CCW)), TowerLamp 신호(Y0B(red), Y0C(yellow), Y0D(green)), Loader 신호(Y0E)

public class mxcomponet : MonoBehaviour
{
    ActUtlType64 mxComponent;
    public bool isConnected = false;
    public float updateInterval = 1;


    // Lifecycle 함수 중 가장 빨리 실행.
    private void Awake()
    {
        mxComponent = new ActUtlType64();
    }

    public void OnOpenBtnClkEvent()
    {
        int iRet = mxComponent.Open();

        if(iRet != 0)
        {
            string error = Convert.ToString(iRet, 16);
            Debug.LogError(error);  // Unity console 창에 띄움.

            return;
        }
        else
        {
            isConnected = true;

            Debug.Log("PLC가 연결되었습니다.");
        }

    }

    public void OnCloseBtnClkEvent()
    {
        if(!isConnected)
        {
            Debug.LogAssertion("PLC 연결을 확인해주세요.");

            return;
        }

        int iRet = mxComponent.Close();

        if (iRet != 0)
        {
            string error = Convert.ToString(iRet, 16);
            Debug.LogError(error);  // Unity console 창에 띄움.

            return;
        }
        else
        {
            isConnected = false;

            Debug.Log("PLC가 해제되었습니다.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
