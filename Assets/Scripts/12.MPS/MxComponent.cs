#define Master

using ActUtlType64Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;


// 목표 : UI(연결, 연결해제) 버튼을 누르면 실시간으로 PLC에 데이터를 요청하고 쓴다.
// 속성 : mxComponent 객체변수, 요청하는 기능, 쓰기 기능
// PLC에 요청할 값들 : SOL, LS(limit sensor), SENSOR, CONVEYOR, TOWERLAMP, Loader
// X(input 신호) : LS신호(X0~7), Sensor 신호(X8, x9,x0a(Loader))
// Y(output 신호) : Sol 신호(Y0~Y7), Conveyor 신호(Y8(on/off), Y9(CW), Y0A(CCW)), TowerLamp 신호(Y0B(red), Y0C(yellow), Y0D(green)), Loader 신호(Y0E)

public class MxComponent : MonoBehaviour
{
    public static MxComponent Instance;  // 싱글턴 패턴 : 한 scene에 MxComponent 객체가 하나만 있어야함.

    public object lockObj = new object();

    // COM참조 추가 DLL추가(STA) -> 생성된 스레드와 동일한 스레드에서만 메서드를 호출이 가능(STA: 단일 스레드 어파트먼트)
    ActUtlType64 mxComponent;

    [Header("PLC 정보")]
    public bool isConnected = false;
    [Tooltip("에러메시지가 표시됩니다.")]
    public int iRet = 0;
    List<bool[]> yOutputs = new List<bool[]>();
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
    [Tooltip("근접센서(Proximity Sensor)를 연결해주세요.")]
    public MPS.Sensor pSensor; // Proximity Sensor
    [Tooltip("금속센서(Metal Sensor)를 연결해주세요.")]
    public MPS.Sensor mSensor; // Metal Sensor
    public Loader loader; // 물체 로딩하는 기능 + 센서기능

    Stopwatch stopwatch = new Stopwatch(); // 스탑워치 인스턴스
    StringBuilder sb = new StringBuilder(); // 문자열을 만들때 사용되는 최적화 클래스


    // Lifecycle 함수 중 가장 빨리 실행.
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void OnOpenBtnClkEvent()
    {
        isConnected = true;
        Debug.Log("서버에 연결되었습니다.");

#if Master                                              // 전처리

        //StartCoroutine(CoUpdatePLCData());
        Task.Run(UpdatePLCDataAsync);

        Task.Run(MPSwithFirebase.FirebaseDBManager.Instance.UpdatePLCDataAsync);

        Debug.Log("PLC가 연결되었습니다.");

#elif Slave
        yDeviceBlocks = new int[yOutputBlockCount];
        xDeviceBlocks = new int[xInputBlockCount];

        Task.Run(MPSwithFirebase.FirebaseDBManager.Instance.ReadPLCDataAsync);

        StartCoroutine(CoApplyOutputSignals());
#endif
    }

    IEnumerator CoApplyOutputSignals()
    {
        while(isConnected)
        {
            yOutputs = ConvertDecimalToBinary(yDeviceBlocks);

            ApplyOutputSignals(yOutputs);

            yield return new WaitForEndOfFrame();
        }
    }

    public void OnCloseBtnClkEvent()
    {
        if (!isConnected)
        {
            Debug.LogWarning("PLC를 우선 연결해주세요.");

            return;
        }

        isConnected = false;

        Debug.Log("PLC연결이 해지되었습니다.");
    }

    async void UpdatePLCDataAsync()
    {
        mxComponent = new ActUtlType64();
        mxComponent.ActLogicalStationNumber = 0;

        iRet = mxComponent.Open();

        xDeviceBlocks = new int[xInputBlockCount];
        yDeviceBlocks = new int[yOutputBlockCount];

        while (isConnected)
        {
            //상호배제(Mutex)로 공유자원에 여러 스레드가 접근하는 것을 순서화 해준다.
            lock(lockObj)
            {
                ReadDeviceBlock(mxComponent, yOutputStartDevice, yOutputBlockCount);

                WriteDeviceBlock(mxComponent, xInputStartDevice, xInputBlockCount);
            }

            int interval = Convert.ToInt32(updateInterval);
            await Task.Delay(interval);
        }

        iRet = mxComponent.Close();
    }

    public int[] yDeviceBlocks;

    // Y(output 신호) : Sol 신호(Y0~Y7), Conveyor 신호(Y8(on/ off), Y9(CW), Y0A(CCW), Y0B(STOP)), TowerLamp 신호(Y0C(red), Y0D(yellow), Y0E(green)), Loader 신호(Y0E)
    private void ReadDeviceBlock(ActUtlType64 mxObject, string _yOutputStartDevice, int _yOutputBlockCount)
    {
        // 10진수 -> 2진수 변환
        iRet = mxObject.ReadDeviceBlock(_yOutputStartDevice, _yOutputBlockCount, out yDeviceBlocks[0]);

        CheckError(iRet);

        yOutputs = new List<bool[]>();

        for (int i = 0; i < yDeviceBlocks.Length; i++)
        {
            bool[] block = new bool[16];

            yOutputs.Add(block);
        }

        yOutputs = ConvertDecimalToBinary(yDeviceBlocks);

        ApplyOutputSignals(yOutputs);

        // 내부함수
        //void ApplyOutputSignals(List<bool[]> yOutputs)
        //{
        //    cylinders[0].isForwardSignal = yOutputs[0][0];  // X0
        //    cylinders[0].isBackwardSignal = yOutputs[0][1]; // X1
        //    cylinders[1].isForwardSignal = yOutputs[0][2];  // X2
        //    cylinders[1].isBackwardSignal = yOutputs[0][3]; // X3
        //    cylinders[2].isForwardSignal = yOutputs[0][4];  // X4
        //    cylinders[2].isBackwardSignal = yOutputs[0][5]; // X5
        //    cylinders[3].isForwardSignal = yOutputs[0][6];  // X6
        //    cylinders[3].isBackwardSignal = yOutputs[0][7]; // X7
        //    conveyor.isConvOnOffSignal = yOutputs[0][8];    // X8
        //    conveyor.isCWSignal = yOutputs[0][9];           // X9
        //    conveyor.isCCWSignal = yOutputs[0][10];         // X0A
        //    conveyor.isCCWSignal = yOutputs[0][11];         // X0A
        //    towerLamp.isRedSignal = yOutputs[0][12];        // X0B
        //    towerLamp.isYelSignal = yOutputs[0][13];        // X0C
        //    towerLamp.isGrnSignal = yOutputs[0][14];        // X0D
        //    // loader.isLoadedSignal = yOutputs[0][14];        // X0E
        //}
    }

    private void ApplyOutputSignals(List<bool[]> yOutputs)
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


    public int[] xDeviceBlocks;

    // 신호들을 10진수로 변환 후 넣어주기.
    // X(input 신호) : LS신호(X0~7), Sensor 신호(X8(근접P), x9(금속M) ,x0a(Loader))
    private void WriteDeviceBlock(ActUtlType64 mxObject, string _xInputStartDevice, int _xInputBlockCount)
    {
        xDeviceBlocks = new int[_xInputBlockCount];

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
        xDeviceBlocks[0] = decimalX;

        iRet = mxObject.WriteDeviceBlock(_xInputStartDevice, _xInputBlockCount, ref xDeviceBlocks[0]);
    }

    // WriteDeviceBlock(가상의 장비에 있는 X디바이스를 실제 센서로 가정하여 사용)
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
