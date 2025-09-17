using UnityEngine;
using ActUtlType64Lib;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;

// 목표: UI(연결, 연결해지)버튼을 누르면 실시간으로 PLC에 데이터를 요청하고, 쓴다.
// 속성: mxComponent 객체변수, 요청하는 기능, 쓰기 기능
// PLC에 요청할 값들: SOL 신호들, LS 신호들, Sensor 신호들, Conveyor 신호들, Towerlamp 신호들
// X디바이스(input 신호, 1블록)
// - LS 신호들(X0, X1, X2, X3, X4, X5, X6, X7)
// - Sensor 신호들(X8(근접), X9(금속), X0A(loader 감지신호))

// Y(output 신호, 1블록)
// - SOL 신호들(Y0, Y1, Y2, Y3, Y4, Y5, Y6, Y7)
// - Conveyor 신호들(Y8(on/off), Y9(cw), Y0A(ccw))
// - Towerlamp 신호들(Y0B(red), Y0C(yellow), Y0D(green))
// - loader 신호(Y0E)
public class MxComponent : MonoBehaviour
{
    // COM참조 추가 DLL추가(STA) -> 생성된 스레드와 동일한 스레드에서만 메서드를 호출이 가능(STA: 단일 스레드 어파트먼트)
    ActUtlType64 mxComponent;

    [Header("PLC 정보")]
    public bool isConnected = false;
    [Tooltip("에러메시지가 표시됩니다.")]
    public int iRet = 0;
    List<bool[]> yOutput;
    public float updateInterval = 1f;
    public string xInputStartDevice = "X0";
    public int xInputBlockCount = 1;
    public string yOutputStartDevice = "Y0";
    public int yOutputBlockCount = 1;

    [Header("Output")]
    public List<Cylinder> cylinders;
    public MPS.Conveyor conveyor;
    public TowerLamp towerLamp;

    // Unity Attribute(속성)
    [Header("Input")]
    [Tooltip("근접센서(Proximity Sensor)를 연결해주세요.")]
    public MPS.Sensor pSensor; // Proximity Sensor
    [Tooltip("금속센서(Metal Sensor)를 연결해주세요.")]
    public MPS.Sensor mSensor; // Metal Sensor
    public Loader loader; // 물체 로딩하는 기능 + 센서기능

    Stopwatch stopwatch = new Stopwatch(); // 스탑워치 인스턴스
    StringBuilder sb = new StringBuilder(); // 문자열을 만들때 사용되는 최적화 클래스

    public void OnOpenBtnClkEvent()
    {
        isConnected = true;

        Task.Run(UpdatePLCDataAsync);

        StartCoroutine(CoShowErrMsg());

        Debug.Log("PLC가 연결되었습니다.");
    }

    public void OnCloseBtnClkEvent()
    {
        if(!isConnected)
        {
            Debug.LogWarning("PLC를 우선 연결해주세요.");

            return;
        }

        isConnected = false;

        Debug.Log("PLC연결이 해지되었습니다.");
    }

    // 서브 스레드에서 PLC 데이터 업데이트
    async void UpdatePLCDataAsync()
    {
        mxComponent = new ActUtlType64();
        mxComponent.ActLogicalStationNumber = 0;

        iRet = mxComponent.Open();

        while (isConnected)
        {
            ReadDeviceBlock(mxComponent, yOutputStartDevice, yOutputBlockCount);

            WriteDeviceBlock(mxComponent, xInputStartDevice, xInputBlockCount);

            int interval = Convert.ToInt32(updateInterval);
            await Task.Delay(interval);
        }

        iRet = mxComponent.Close();
    }

    IEnumerator CoShowErrMsg()
    {
        while (isConnected)
        {
            yield return new WaitUntil(() => iRet != 0);

            CheckError(iRet);
        }
    }

    private void ReadDeviceBlock(ActUtlType64 mxObject, string _yInputStartDevice, int _yInputBlockCount)
    {
        // 10진수 -> 2진수
        // { 33, 55, 500 } -> { 0011000000110000, 0011000000110000, 0011000000110000 }
        int[] data = new int[_yInputBlockCount];
        iRet = mxObject.ReadDeviceBlock(_yInputStartDevice, _yInputBlockCount, out data[0]);

        yOutput = new List<bool[]>();
        for (int i = 0; i < data.Length; i++)
        {
            bool[] block = new bool[16];

            yOutput.Add(block);
        }

        yOutput = ConvertDecimalToBinary(data);

        ApplyOutputSignals(yOutput);
    }

    void ApplyOutputSignals(List<bool[]> yOutput)
    {
        // 각 GameObject의 스크립트에 연결 (첫번째 블록의 각 비트의 작동여부 0 or 1)
        // Y(output 신호, 1블록)
        // - SOL 신호들(Y0, Y1, Y2, Y3, Y4, Y5, Y6, Y7)
        // - Conveyor 신호들(Y8(on/off), Y9(cw), Y0A(ccw))
        // - Towerlamp 신호들(Y0B(red), Y0C(yellow), Y0D(green))
        // - loader 신호(Y0E)
        cylinders[0].isForwardSignal = yOutput[0][0];   // X0
        cylinders[0].isBackwardSignal = yOutput[0][1];   // X1
        cylinders[1].isForwardSignal = yOutput[0][2];   // X2
        cylinders[1].isBackwardSignal = yOutput[0][3];   // X3
        cylinders[2].isForwardSignal = yOutput[0][4];   // X4
        cylinders[2].isBackwardSignal = yOutput[0][5];   // X5
        cylinders[3].isForwardSignal = yOutput[0][6];   // X6
        cylinders[3].isBackwardSignal = yOutput[0][7];   // X7
        conveyor.isConvOnOffSignal = yOutput[0][8];   // X8
        conveyor.isCWSignal = yOutput[0][9];   // X9
        conveyor.isCCWSignal = yOutput[0][10];  // X0A
        towerLamp.isRedSignal = yOutput[0][11];  // X0B
        towerLamp.isYellowSignal = yOutput[0][12];  // X0C
        towerLamp.isGreenSignal = yOutput[0][13];  // X0D
        //loader.isLoadedSignal = yOutput[0][14];  // X0E 
    }

    private void WriteDeviceBlock(ActUtlType64 mxObject, string _xInputStartDevice, int _xInputBlockCount)
    {
        int[] data = new int[_xInputBlockCount];

        // 신호들을 10진수로 변환
        // X디바이스(input 신호, 1블록)
        // - LS 신호들(X0, X1, X2, X3, X4, X5, X6, X7)
        // - Sensor 신호들(X8(근접), X9(금속), X0A(loader 감지신호))
        char limitSW0 = (cylinders[0].isForwardSWON     == true) ? '1' : '0'; // 삼항연산자
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

        string totalBinaryStr = $"{limitSW0}{limitSW1}{limitSW2}{limitSW3}{limitSW4}" +
                                $"{limitSW5}{limitSW6}{limitSW7}{pSensorValue}" +
                                $"{mSensorValue}{loaderSensorValue}";

        totalBinaryStr = new string(totalBinaryStr.Reverse().ToArray());

        // 문자열 -> int로
        // 0000 0010 -> 2
        int decimalX = Convert.ToInt32(totalBinaryStr, 2);
        data[0] = decimalX;

        iRet = mxObject.WriteDeviceBlock(_xInputStartDevice, _xInputBlockCount, ref data[0]);
    }


    // WriteDeviceBlock(가상의 장비에 있는 X디바이스를 실제 센서로 가정하여 사용)
    // 10진수 -> 2진수 bool 배열로 변환하는 메서드
    private List<bool[]> ConvertDecimalToBinary(int[] data)
    {
        List<bool[]> result = new List<bool[]>();

        // { 33, 55, 500 }
        for (int i = 0; i < data.Length; i++)
        {
            bool[] block = new bool[16]; // 2진수 임시공간

            for (int j = 0; j < block.Length; j++)
            {
                // 비트연산: 0000 0000 0000 0001  bit shift & 비교
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
