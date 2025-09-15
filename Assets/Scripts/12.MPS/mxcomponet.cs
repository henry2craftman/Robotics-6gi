using UnityEngine;
using ActUtlType64Lib;
using System;
using System.Collections;
using System.Collections.Generic;


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
    public float updateInterval = 1f;
    public string xInputStartDevice = "X0";
    public int xInputBlockCount = 1;
    public string yOutputStartDevice = "Y0";
    public int yOutputBlockCount = 1;

    [Header("���� ���� ����")]
    public List<Cylinder> cylinders;


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
            ReadDeviceBlock(xInputStartDevice, xInputBlockCount);

            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void ReadDeviceBlock(string _xInputStartDevice, int _xInputBlockCount)
    {
        // 10���� -> 2���� ��ȯ
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

        // �� GameObject�� ��ũ��Ʈ�� ����
        cylinders[0].isForwardSignal = yOutputs[0][0];
        cylinders[0].isBackwardSignal = yOutputs[0][1];
        cylinders[1].isForwardSignal = yOutputs[0][2];
        cylinders[1].isBackwardSignal = yOutputs[0][3];
        cylinders[2].isForwardSignal = yOutputs[0][4];
        cylinders[2].isBackwardSignal = yOutputs[0][5];
        cylinders[3].isForwardSignal = yOutputs[0][6];
        cylinders[3].isBackwardSignal = yOutputs[0][7];
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
