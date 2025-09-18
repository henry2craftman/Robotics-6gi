using ActUtlType64Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;


// ��ǥ : UI(����, ��������) ��ư�� ������ �ǽð����� PLC�� �����͸� ��û�ϰ� ����.
// �Ӽ� : mxComponent ��ü����, ��û�ϴ� ���, ���� ���
// PLC�� ��û�� ���� : SOL, LS(limit sensor), SENSOR, CONVEYOR, TOWERLAMP, Loader
// X(input ��ȣ) : LS��ȣ(X0~7), Sensor ��ȣ(X8, x9,x0a(Loader))
// Y(output ��ȣ) : Sol ��ȣ(Y0~Y7), Conveyor ��ȣ(Y8(on/off), Y9(CW), Y0A(CCW)), TowerLamp ��ȣ(Y0B(red), Y0C(yellow), Y0D(green)), Loader ��ȣ(Y0E)

public class mxcomponet : MonoBehaviour
{
    ActUtlType64 mxComponent;

    [Header("PLC ����")]
    public bool isConnected = false;
    public List<bool[]> yOutputs;
    public float updateInterval = 0.5f;
    public string xInputStartDevice = "X0";
    public int xInputBlockCount = 1;
    public string yOutputStartDevice = "Y0";
    public int yOutputBlockCount = 1;

    [Header("���� ���� ����(Output)")]
    public List<Cylinder> cylinders;
    public MPS.Conveyor conveyor;
    public TowerLamp towerLamp;

    [Header("���� ���� ����(Input)")]
    public MPS.Sensor pSensor;              // Proximity Sensor
    public MPS.Sensor mSensor;              // Metal Sensor
    public Loader loader;                   // �ε��ϴ� ��� + �������

    StringBuilder sb = new StringBuilder(); // ���ڿ��� ���鶧 ���Ǵ� ����ȭ Ŭ����


    // Lifecycle �Լ� �� ���� ���� ����.
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
            Debug.LogError(error);  // Unity console â�� ���.

            return;
        }
        else
        {
            isConnected = true;

            StartCoroutine(CoUpdatePLCData());

            Debug.Log("PLC�� ����Ǿ����ϴ�.");
        }

    }

    public void OnCloseBtnClkEvent()
    {
        if(!isConnected)
        {
            Debug.LogAssertion("PLC ������ Ȯ�����ּ���.");

            return;
        }

        int iRet = mxComponent.Close();

        if (iRet != 0)
        {
            string error = Convert.ToString(iRet, 16);
            Debug.LogError(error);  // Unity console â�� ���.

            return;
        }
        else
        {
            isConnected = false;

            Debug.Log("PLC�� �����Ǿ����ϴ�.");
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


    // Y(output ��ȣ) : Sol ��ȣ(Y0~Y7), Conveyor ��ȣ(Y8(on/ off), Y9(CW), Y0A(CCW), Y0B(STOP)), TowerLamp ��ȣ(Y0C(red), Y0D(yellow), Y0E(green)), Loader ��ȣ(Y0E)
    private void ReadDeviceBlock(string _yOutputStartDevice, int _yOutputBlockCount)
    {
        // 10���� -> 2���� ��ȯ
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

        // �����Լ�
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

    // ��ȣ���� 10������ ��ȯ �� �־��ֱ�.
    // X(input ��ȣ) : LS��ȣ(X0~7), Sensor ��ȣ(X8(����P), x9(�ݼ�M) ,x0a(Loader))
    private void WriteDeviceBlock(string _xInputStartDevice, int _xInputBlockCount)
    {
        int[] data = new int[_xInputBlockCount];

        char limitSW0 = (cylinders[0].isForwardSWON     == true) ? '1' : '0'; // ���� ������
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

        // ���ڿ� -> int�� �迭�� ��ȯ
        int decimalX = Convert.ToInt32(newTotal, 2);
        data[0] = decimalX;

        mxComponent.WriteDeviceBlock(_xInputStartDevice, _xInputBlockCount, ref data[0]);
    }

    // 10���� -> 2���� bool �迭�� ��ȯ�ϴ� �޼���
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
