using UnityEngine;
using ActUtlType64Lib;
using System;
using System.Collections;
using System.Collections.Generic;


// 목표 : UI(연결, 연결해제) 버튼을 누르면 실시간으로 PLC에 데이터를 요청하고 쓴다.
// 속성 : mxComponent 객체변수, 요청하는 기능, 쓰기 기능
// PLC에 요청할 값들 : SOL, LS(limit sensor), SENSOR, CONVEYOR, TOWERLAMP, Loader
// X(input 신호) : LS신호(X0~7), Sensor 신호(X8, x9,x0a(Loader))
// Y(output 신호) : Sol 신호(Y0~Y7), Conveyor 신호(Y8(on/off), Y9(CW), Y0A(CCW)), TowerLamp 신호(Y0B(red), Y0C(yellow), Y0D(green)), Loader 신호(Y0E)

public class mxcomponet : MonoBehaviour
{
    ActUtlType64 mxComponent;

    [Header("PLC 정보")]
    public bool isConnected = false;
    public List<bool[]> yOutputs;
    public float updateInterval = 1f;
    public string xInputStartDevice = "X0";
    public int xInputBlockCount = 1;
    public string yOutputStartDevice = "Y0";
    public int yOutputBlockCount = 1;

    [Header("가상 설비 참조")]
    public List<Cylinder> cylinders;


    // Lifecycle 함수 중 가장 빨리 실행.
    private void Awake()
    {
        mxComponent = new ActUtlType64();
        mxComponent.ActLogicalStationNumber = 0; // default : 0
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

            StartCoroutine(CoUpdatePLCData());

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

    IEnumerator CoUpdatePLCData()
    {
        while(true)
        {
            ReadDeviceBlock(xInputStartDevice, xInputBlockCount);

            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void ReadDeviceBlock(string _xInputStartDevice, int _xInputBlockCount)
    {
        // 10진수 -> 2진수 변환
        int[] data = new int[_xInputBlockCount];
        int iRet = mxComponent.ReadDeviceBlock(_xInputStartDevice, _xInputBlockCount, out data[0]);

        CheckError(iRet);

        List<bool[]> yOutputs = new List<bool[]>();

        for(int i =0; i < data.Length; i++)
        {
            bool[] block = new bool[16];

            yOutputs.Add(block);
        }

        yOutputs = ConvertDecimalToBinary(data);

        // 각 GameObject의 스크립트에 연결
        cylinders[0].isForwardSignal = yOutputs[0][0];
        cylinders[0].isBackwardSignal = yOutputs[0][1];
        cylinders[1].isForwardSignal = yOutputs[0][2];
        cylinders[1].isBackwardSignal = yOutputs[0][3];
        cylinders[2].isForwardSignal = yOutputs[0][4];
        cylinders[2].isBackwardSignal = yOutputs[0][5];
        cylinders[3].isForwardSignal = yOutputs[0][6];
        cylinders[3].isBackwardSignal = yOutputs[0][7];
    }

    // 10진수 -> 2진수 bool 배열로 변환하는 메서드
    private List<bool[]> ConvertDecimalToBinary(int[] data)
    {
        List<bool[]> result = new List<bool[]>();

        for(int i = 0; i < data.Length; i++)
        {
            bool[] block = new bool[16];

            for(int j = 0; j < block.Length; j++)
            {
                bool isBitSet = ((data[i] & (1 << j)) != 0);
                block[j] = isBitSet;
            }

            result.Add(block);
        }

        return result;
    }

    private static void CheckError(int iRet)
    {
        if (iRet != 0)
        {
            string error = Convert.ToString(iRet, 16);
            Debug.LogError(error);

            return;
        }
    }

    private void OnApplicationQuit()
    {
        OnCloseBtnClkEvent();
    }


}
