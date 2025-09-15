using UnityEngine;
using ActUtlType64Lib;
using System;
using System.Collections.Generic;
using System.Collections;

// 목표: UI(연결, 연결해지)버튼을 누르면 실시간으로 PLC에 데이터를 요청하고, 쓴다.
// 속성: mxComponent 객체변수, 요청하는 기능, 쓰기 기능
// PLC에 요청할 값들: SOL 신호들, LS 신호들, Sensor 신호들, Conveyor 신호들, Towerlamp 신호들
// X(input 신호, 1블록)
// - LS 신호들(X0, X1, X2, X3, X4, X5, X6, X7)
// - Sensor 신호들(X8(근접), X9(금속), X0A(loader 감지신호))
// Y(output 신호, 1블록)
// - SOL 신호들(Y0, Y1, Y2, Y3, Y4, Y5, Y6, Y7)
// - Conveyor 신호들(Y8(on/off), Y9(cw), Y0A(ccw))
// - Towerlamp 신호들(Y0B(red), Y0C(yellow), Y0D(green))
// - loader 신호(Y0E)
public class MxComponent : MonoBehaviour
{
    ActUtlType64 mxComponent;

    [Header("PLC 정보")]
    public bool isConnected = false;
    public List<bool[]> yOutput;
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
        mxComponent.ActLogicalStationNumber = 0; // default: 0
    }

    public void OnOpenBtnClkEvent()
    {
        int iRet = mxComponent.Open();

        if( iRet != 0 )
        {
            string error = Convert.ToString( iRet, 16 );
            Debug.LogWarning( error );

            return;
        }

        isConnected = true;

        StartCoroutine(CoUpdatePLCData());

        Debug.Log("PLC가 연결되었습니다.");
    }

    public void OnCloseBtnClkEvent()
    {
        if(!isConnected)
        {
            Debug.LogWarning("PLC를 우선 연결해주세요.");

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

        Debug.Log("PLC연결이 해지되었습니다.");
    }

    private void Test()
    {
        ReadDeviceBlock(xInputStartDevice, xInputBlockCount);
    }

    IEnumerator CoUpdatePLCData()
    {
        while (isConnected)
        {
            ReadDeviceBlock(xInputStartDevice, xInputBlockCount);

            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void ReadDeviceBlock(string _xInputStartDevice, int _xInputBlockCount)
    {
        // 10진수 -> 2진수
        // { 33, 55, 500 } -> { 0011000000110000, 0011000000110000, 0011000000110000 }
        int[] data = new int[_xInputBlockCount];
        int iRet = mxComponent.ReadDeviceBlock(_xInputStartDevice, _xInputBlockCount, out data[0]);

        CheckError(iRet);

        yOutput = new List<bool[]>();
        for (int i = 0; i < data.Length; i++)
        {
            bool[] block = new bool[16];

            yOutput.Add(block);
        }

        yOutput = ConvertDecimalToBinary(data);

        // 각 GameObject의 스크립트에 연결
        cylinders[0].isForwardSignal  = yOutput[0][0];   // X0
        cylinders[0].isBackwardSignal = yOutput[0][1];   // X1
        cylinders[1].isForwardSignal  = yOutput[0][2];   // X2
        cylinders[1].isBackwardSignal = yOutput[0][3];   // X3
        cylinders[2].isForwardSignal  = yOutput[0][4];   // X4
        cylinders[2].isBackwardSignal = yOutput[0][5];   // X5
        cylinders[3].isForwardSignal  = yOutput[0][6];   // X6
        cylinders[3].isBackwardSignal = yOutput[0][7];   // X7
    }

    // 10진수 -> 2진수 bool 배열로 변환하는 메서드
    private List<bool[]> ConvertDecimalToBinary(int[] data)
    {
        List<bool[]> result = new List<bool[]>();

        // { 33, 55, 500 }
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

            return;
        }
    }

    private void OnApplicationQuit()
    {
        OnCloseBtnClkEvent();
    }
}
