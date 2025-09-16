using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ActUtlType64Lib;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

// MxComponent�� ��ǥ: UI(����, ��������)��ư�� ������ �ǽð����� PLC�� �����͸� ��û�ϰ� ����.
// �Ӽ� : mxComponent ��ü(�������̽�)����, ��û�ϴ� ���, ���� ���,  
// plc�� ��û�� ���� : sol��ȣ��, LS ��ȣ��, ������ȣ��,Lamp ��ȣ��, Conveyor ��ȣ��.
// X����̽�(plc input):  x00~x07 - LS��ȣ�� / x08,x09, x0a(��ǰ ��ġ-�δ�)������ȣ��/
// Y����̽�(plc output): y00~y07 - sol ��ȣ�� / �����̾�(y08(onoff),y09(cw), y0a(ccw)) / y0b,y0c,y0d : RYG - ������ȣ
//                        y0e(��ǰ�����ִ� ��ȣ-�δ�)

public class MxComponent : MonoBehaviour
{
    ActUtlType64 mxComponent;

    [Header("PLC info")]
    public bool isConnected = false;
    public float updateInterval = 1f;
    public string xInputStartDevice = "X0";
    public int xInputBlockCount = 1;
    public string yOuputStartDevice = "Y0";
    public int yOuputBlockCount = 1;

    [Header("Virtual Machine(PLC Output)")]
    public List<Cylinder> cylinders;
    public MPS.Conveyor conveyor;
    public TowerLamp towerLamp;

    [Header("Virtual(PLC Input)")]
    [Tooltip("PSensor Connect Please")]
    public MPS.Sensor pSensor; //Proximity Sensor
    [Tooltip("MSensor Connect Please")]
    public MPS.Sensor mSensor; //Metal Sensor
    [Tooltip("LoaderSensor Connect Please")]
    public Loader loader;

    StringBuilder sb = new StringBuilder();

    // ������ ����Ŭ �Լ� �� ���� ���� ����.
    private void Awake()
    {
        mxComponent = new ActUtlType64();
        mxComponent.ActLogicalStationNumber = 0;
    }
    private void Start()
    {
        OnOpenBtnClkEvent();
        //Invoke("Test", 3); //3�ʸ��� �ҷ����� �߾���.

    }

    private void Test()
    {
        ReadDeviceBlock(xInputStartDevice, xInputBlockCount);
    }

    IEnumerator CoUpdatePLCData()
    {
        while (true)
        {
            ReadDeviceBlock(yOuputStartDevice, yOuputBlockCount);

            WriteDeviceBlock(xInputStartDevice, xInputBlockCount);

            yield return new WaitForSeconds(updateInterval);
        }
    }

    public void OnOpenBtnClkEvent()
    {
        int iRet = mxComponent.Open();
        if (iRet != 0)
        {
            string error = Convert.ToString(iRet, 16);
            Debug.LogWarning(error);

            return;
        }

        isConnected = true;
        StartCoroutine(CoUpdatePLCData());
        Debug.Log("PLC�� ����Ǿ����ϴ�.");
    }
    public void OnCloseBtnClkEvent()
    {
        if (!isConnected)
        {
            Debug.LogAssertion("PLC�� �켱 �������ּ���.");
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
        Debug.Log("PLC ������ �����Ǿ����ϴ�.");

    }

    private void ReadDeviceBlock(string _yOuputStartDevice, int _yOuputBlockCount)
    {
        // {33,55,500} -> {0011000000011000.0011000000011000.0011000000011000} ... 

        int[] data = new int[_yOuputBlockCount];
        int iRet = mxComponent.ReadDeviceBlock(_yOuputStartDevice, _yOuputBlockCount, out data[0]);

        CheckError(iRet);

        List<bool[]> yOutputs = new List<bool[]>();

        bool[] block = new bool[16];
        for (int i = 0; i < data.Length; i++)
        {
            yOutputs.Add(block);
        }
        yOutputs = ConvertDecimalToBinary(data);

        ApplyOutSignals(yOutputs);

        void ApplyOutSignals(List<bool[]> yOutputs)
        {
            cylinders[0].isForwardSignal = yOutputs[0][0];  //X0
            cylinders[0].isBackwardSignal = yOutputs[0][1]; //X1
            cylinders[1].isForwardSignal = yOutputs[0][2];  //X2
            cylinders[1].isBackwardSignal = yOutputs[0][3]; //X3
            cylinders[2].isForwardSignal = yOutputs[0][4];  //X4
            cylinders[2].isBackwardSignal = yOutputs[0][5]; //X5
            cylinders[3].isForwardSignal = yOutputs[0][6];  //X6
            cylinders[3].isBackwardSignal = yOutputs[0][7]; //X7

            conveyor.isConvOnOffSignal = yOutputs[0][8];    //X8
            conveyor.isCWSignal = yOutputs[0][9];           //X9
            conveyor.isCCWSignal = yOutputs[0][10];        //X0a

            towerLamp.isRedSignal = yOutputs[0][11];       //X0b
            towerLamp.isYellowSignal = yOutputs[0][12];    //X0c
            towerLamp.isGreenSignal = yOutputs[0][13];     //X0d

            //loader.isLoadSignal = yOutputs[0][14];         //X0e
        }
    }

    // ���� y��ȣ�� ����

    // 10���� -> 2���� bool �迭�� ��ȯ�ϴ� �޼���
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

    private void WriteDeviceBlock(string _xInputStartDevice, int _xInputBlockCount)
    {
        int[] data = new int[_xInputBlockCount];
        bool[] binarySignals = new bool[16];

        char limitSW0 = (cylinders[0].isForwardSWON == true) ? '1' : '0'; // ���׿�����
        char limitSW1 = (cylinders[0].isBackwardSWON == true) ? '1' : '0'; ;
        char limitSW2 = (cylinders[1].isForwardSWON == true) ? '1' : '0'; ;
        char limitSW3 = (cylinders[1].isBackwardSWON == true) ? '1' : '0'; ;
        char limitSW4 = (cylinders[2].isForwardSWON == true) ? '1' : '0'; ;
        char limitSW5 = (cylinders[2].isBackwardSWON == true) ? '1' : '0'; ;
        char limitSW6 = (cylinders[3].isForwardSWON == true) ? '1' : '0'; ;
        char limitSW7 = (cylinders[3].isBackwardSWON == true) ? '1' : '0'; ;

        char pSensorValue = (pSensor.isActive == true) ? '1' : '0'; ;
        char mSensorValue = (mSensor.isActive == true) ? '1' : '0'; ;
        char LoaderSensorValue = (loader.isLoadSignal == true) ? '1' : '0'; ;

        string totalBinaryStr = $"{LoaderSensorValue}{mSensorValue}{pSensorValue}{limitSW7}{limitSW6}{limitSW5}{limitSW4}{limitSW3}{limitSW2}{limitSW1}{limitSW0}";
        //totalBinaryStr = new string(totalBinaryStr.Reverse().ToArray()); ������ �ϰ� ���� ���.

        //���ڿ� -> int��!
        int decimalX = Convert.ToInt32(totalBinaryStr,2);
        data[0] = decimalX;

        mxComponent.WriteDeviceBlock(_xInputStartDevice, _xInputBlockCount, ref data[0]);

    }

    private static void CheckError(int iRet)
    {
        if (iRet != 0)
        {
            string result = Convert.ToString(iRet, 16);
            Debug.LogError(result);
        }
    }

    private void OnApplicationQuit() // �ı��ڰ� ���⶧���� ���.
    {
        OnCloseBtnClkEvent();
    }
}



