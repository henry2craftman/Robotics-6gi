using ActUtlType64Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;


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
    public float updateInterval = 0.5f;
    public string xInputStartDevice = "X0";
    public int xInputBlockCount = 1;
    public string yOutputStartDevice = "Y0";
    public int yOutputBlockCount = 1;

    [Header("가상 설비 참조(Output)")]
    public List<Cylinder> cylinders;
    public MPS.Conveyor conveyor;
    public TowerLamp towerLamp;

    [Header("가상 설비 참조(Input)")]
    public MPS.Sensor pSensor;              // Proximity Sensor
    public MPS.Sensor mSensor;              // Metal Sensor
    public Loader loader;                   // 로딩하는 기능 + 센서기능

    StringBuilder sb = new StringBuilder(); // 문자열을 만들때 사용되는 최적화 클래스


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
            ReadDeviceBlock(yOutputStartDevice, yOutputBlockCount);

            WriteDeviceBlock(xInputStartDevice, xInputBlockCount);

            yield return new WaitForSeconds(updateInterval);
        }
    }


    // Y(output 신호) : Sol 신호(Y0~Y7), Conveyor 신호(Y8(on/ off), Y9(CW), Y0A(CCW), Y0B(STOP)), TowerLamp 신호(Y0C(red), Y0D(yellow), Y0E(green)), Loader 신호(Y0E)
    private void ReadDeviceBlock(string _yOutputStartDevice, int _yOutputBlockCount)
    {
        // 10진수 -> 2진수 변환
        int[] data = new int[_yOutputBlockCount];
        int iRet = mxComponent.ReadDeviceBlock(_yOutputStartDevice, _yOutputBlockCount, out data[0]);

        CheckError(iRet);

        List<bool[]> yOutputs = new List<bool[]>();

        for (int i = 0; i < data.Length; i++)
        {
            bool[] block = new bool[16];

            yOutputs.Add(block);
        }

        yOutputs = ConvertDecimalToBinary(data);

        ApplyOutputSignals(yOutputs);

        // 내부함수
        void ApplyOutputSignals(List<bool[]> yOutputs)
        {
            cylinders[0].isForwardSignal = yOutputs[0][0];  // X0
            cylinders[0].isBackwardSignal = yOutputs[0][1]; // X1
            cylinders[1].isForwardSignal = yOutputs[0][2];  // X2
            cylinders[1].isBackwardSignal = yOutputs[0][3]; // X3
            cylinders[2].isForwardSignal = yOutputs[0][4];  // X4
            cylinders[2].isBackwardSignal = yOutputs[0][5]; // X5
            cylinders[3].isForwardSignal = yOutputs[0][6];  // X6
            cylinders[3].isBackwardSignal = yOutputs[0][7]; // X7
            conveyor.isConvOnOffSignal = yOutputs[0][8];    // X8
            conveyor.isCWSignal = yOutputs[0][9];           // X9
            conveyor.isCCWSignal = yOutputs[0][10];         // X0A
            conveyor.isCCWSignal = yOutputs[0][11];         // X0A
            towerLamp.isRedSignal = yOutputs[0][12];        // X0B
            towerLamp.isYelSignal = yOutputs[0][13];        // X0C
            towerLamp.isGrnSignal = yOutputs[0][14];        // X0D
            // loader.isLoadedSignal = yOutputs[0][14];        // X0E
        }
    }

    // 신호들을 10진수로 변환 후 넣어주기.
    // X(input 신호) : LS신호(X0~7), Sensor 신호(X8(근접P), x9(금속M) ,x0a(Loader))
    private void WriteDeviceBlock(string _xInputStartDevice, int _xInputBlockCount)
    {
        int[] data = new int[_xInputBlockCount];

        char limitSW0 = (cylinders[0].isForwardSWON     == true) ? '1' : '0'; // 삼항 연산자
        char limitSW1 = (cylinders[0].isBackSWON        == true) ? '1' : '0';
        char limitSW2 = (cylinders[1].isForwardSWON     == true) ? '1' : '0';
        char limitSW3 = (cylinders[1].isBackSWON        == true) ? '1' : '0';
        char limitSW4 = (cylinders[2].isForwardSWON     == true) ? '1' : '0';
        char limitSW5 = (cylinders[2].isBackSWON        == true) ? '1' : '0';
        char limitSW6 = (cylinders[3].isForwardSWON     == true) ? '1' : '0';
        char limitSW7 = (cylinders[3].isBackSWON        == true) ? '1' : '0';
        char pSensorValue = (pSensor.isActive           == true) ? '1' : '0';
        char mSensorValue = (mSensor.isActive           == true) ? '1' : '0';
        char loaderSensorValue = (loader.isLoadedSignal == true) ? '1' : '0';

        string total = $"{limitSW0}{limitSW1}{limitSW2}{limitSW3}{limitSW4}{limitSW5}{limitSW6}{limitSW7}{pSensorValue}{mSensorValue}{loaderSensorValue}";

        string newTotal = new string(total.Reverse().ToArray());

        // 문자열 -> int형 배열로 변환
        int decimalX = Convert.ToInt32(newTotal, 2);
        data[0] = decimalX;

        mxComponent.WriteDeviceBlock(_xInputStartDevice, _xInputBlockCount, ref data[0]);
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



    // Error Check
    private static void CheckError(int iRet)
    {
        if (iRet != 0)
        {
            string error = Convert.ToString(iRet, 16);
            Debug.LogError(error);

            return;
        }
    }

    // PLC OFF
    private void OnApplicationQuit()
    {
        OnCloseBtnClkEvent();
    }


}
