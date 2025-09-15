using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using ActUtlType64Lib;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

// MxComponent�� ��ǥ: UI(����, ��������)��ư�� ������ �ǽð����� PLC�� �����͸� ��û�ϰ� ����.
// �Ӽ� : mxComponent ��ü(�������̽�)����, ��û�ϴ� ���, ���� ���,  
// plc�� ��û�� ���� : sol��ȣ��, LS ��ȣ��, ������ȣ��,Lamp ��ȣ��, Conveyor ��ȣ��.
// X: x00~x07 - LS��ȣ�� / x08,x09, x0a(��ǰ ��ġ-�δ�)������ȣ��/
// Y: y00~y07 - sol ��ȣ�� / �����̾�(y08(onoff),y09(cw), y0a(ccw)) / y0b,y0c,y0d : RYG - ������ȣ
//    y0e(��ǰ�����ִ� ��ȣ-�δ�)

public class MxComponent : MonoBehaviour
{
    ActUtlType64 mxComponent;

    [Header("PLC ����")]
    public bool isConnected = false;
    public float updateInterval = 1f;
    public string xInputStartDevice = "X0";
    public int xInputBlockCount = 1;
    public string yOuputStartDevice = "Y0";
    public int yOuputBlockCount = 1;

    [Header("���� ���� ����")]
    public List<Cylinder> cylinders;

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

