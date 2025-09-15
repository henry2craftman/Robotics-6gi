using UnityEngine;
using ActUtlType64Lib;
using System;


// ��ǥ : UI(����, ��������) ��ư�� ������ �ǽð����� PLC�� �����͸� ��û�ϰ� ����.
// �Ӽ� : mxComponent ��ü����, ��û�ϴ� ���, ���� ���
// PLC�� ��û�� ���� : SOL, LS(limit sensor), SENSOR, CONVEYOR, TOWERLAMP, Loader
// X(input ��ȣ) : LS��ȣ(X0~7), Sensor ��ȣ(X8, x9,x0a(Loader))
// Y(output ��ȣ) : Sol ��ȣ(Y0~Y7), Conveyor ��ȣ(Y8(on/off), Y9(CW), Y0A(CCW)), TowerLamp ��ȣ(Y0B(red), Y0C(yellow), Y0D(green)), Loader ��ȣ(Y0E)

public class mxcomponet : MonoBehaviour
{
    ActUtlType64 mxComponent;
    public bool isConnected = false;
    public float updateInterval = 1;


    // Lifecycle �Լ� �� ���� ���� ����.
    private void Awake()
    {
        mxComponent = new ActUtlType64();
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
