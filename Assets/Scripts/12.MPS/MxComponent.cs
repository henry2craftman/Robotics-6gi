using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using ActUtlType64Lib;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

// MxComponent의 목표: UI(연결, 연결해지)버튼을 누르면 실시간으로 PLC의 데이터를 요청하고 쓴다.
// 속성 : mxComponent 객체(인터페이스)변수, 요청하는 기능, 쓰기 기능,  
// plc에 요청할 값들 : sol신호들, LS 신호들, 센서신호들,Lamp 신호들, Conveyor 신호들.
// X: x00~x07 - LS신호들 / x08,x09, x0a(제품 위치-로더)센서신호들/
// Y: y00~y07 - sol 신호들 / 컨베이어(y08(onoff),y09(cw), y0a(ccw)) / y0b,y0c,y0d : RYG - 램프신호
//    y0e(제품떨궈주는 신호-로더)

public class MxComponent : MonoBehaviour
{
    ActUtlType64 mxComponent;

    [Header("PLC 정보")]
    public bool isConnected = false;
    public float updateInterval = 1f;
    public string xInputStartDevice = "X0";
    public int xInputBlockCount = 1;
    public string yOuputStartDevice = "Y0";
    public int yOuputBlockCount = 1;

    [Header("가상 설비 참조")]
    public List<Cylinder> cylinders;

    // 라이프 사이클 함수 중 가장 빨리 실행.
    private void Awake()
    {
        mxComponent = new ActUtlType64();
        mxComponent.ActLogicalStationNumber = 0;
    }
    private void Start()
    {
        OnOpenBtnClkEvent();
        //Invoke("Test", 3); //3초마다 불러오게 했었다.

    }

    private void Test()
    {
        ReadDeviceBlock(xInputStartDevice, xInputBlockCount);
    }

    IEnumerator CoUpdatePLCData()
    {
        while (true)
        {
            ReadDeviceBlock(xInputStartDevice, xInputBlockCount);
            yield return new WaitForSeconds(updateInterval);
        }
    }

    public void OnOpenBtnClkEvent()
    {
        int iRet = mxComponent.Open();
        if (iRet != 0)
        {
            string error = Convert.ToString(iRet,16);
            Debug.LogWarning(error);

            return;
        }

        isConnected = true;
        StartCoroutine(CoUpdatePLCData());
        Debug.Log("PLC가 연결되었습니다.");
    }
    public void OnCloseBtnClkEvent()
    {
        if (!isConnected)
        {
            Debug.LogAssertion("PLC를 우선 연결해주세요.");
            return;
        }
        int iRet = mxComponent.Close();
        if (iRet != 0)
        {
            string error = Convert.ToString(iRet, 16);
            Debug.LogWarning(error);

            return;
        }

        isConnected = false;
        Debug.Log("PLC 연결이 해제되었습니다.");

    }

    private void ReadDeviceBlock(string _xInputStartDevice, int _xInputBlockCount)
    {
        // {33,55,500} -> {0011000000011000.0011000000011000.0011000000011000} ... 

        int[] data = new int[_xInputBlockCount];
        int iRet = mxComponent.ReadDeviceBlock(_xInputStartDevice, _xInputBlockCount, out data[0]); 

        CheckError(iRet);

        List<bool[]> yOutputs = new List<bool[]>();

        bool[] block = new bool[16];
        for (int i = 0; i < data.Length; i++)
        {
            yOutputs.Add(block);
        }
        yOutputs = ConvertDecimalToBinary(data);
        cylinders[0].isForwardSignal = yOutputs[0][0];  //X0
        cylinders[0].isBackwardSignal = yOutputs[0][1]; //X1
        cylinders[1].isForwardSignal = yOutputs[0][2];  //X2
        cylinders[1].isBackwardSignal = yOutputs[0][3]; //X3
        cylinders[2].isForwardSignal = yOutputs[0][4];  //X4
        cylinders[2].isBackwardSignal = yOutputs[0][5]; //X5
        cylinders[3].isForwardSignal = yOutputs[0][6];  //X6
        cylinders[3].isBackwardSignal = yOutputs[0][7]; //X7


    }

    // 10진수 -> 2진수 bool 배열로 변환하는 메서드
    private List<bool[]> ConvertDecimalToBinary(int[] data)
    {
        List<bool[]> result = new List<bool[]>();

        for (int i = 0; i < data.Length; i++)
        {
            bool[] block = new bool[16];
            for (int j = 0; j < block.Length; j++)
            {
                // 0000 0000 0000 0001
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
            string result = Convert.ToString(iRet, 16);
            Debug.LogError(result);
        }
    }

    private void OnApplicationQuit()
    {
        OnCloseBtnClkEvent();
    }
}

