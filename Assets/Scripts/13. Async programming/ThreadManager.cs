using UnityEngine;
using System.Threading;


// ��ǥ : Thread�� ����� �񵿱� ���α׷� ����
// Ű �Է��� �޾��� ��, �� ���� �Լ��� �����. ���� ������� ���α׷��� �۵��Ѵ�.
public class ThreadManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //1. ������Ȳ
        if(Input.GetKeyDown(KeyCode.Alpha1)) // keyboard 1�� key ���
        {
            Debug.Log("�� �����Լ� ȣ��");
            LaggyFunction();
            Debug.Log("�Լ� ����.");
        }

        //2. �ذ�å
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("�� �����Լ� ȣ��");
            Thread thread = new Thread(LaggyFunction);
            thread.Start();

            Debug.Log("�Լ� ����.");
        }

        Debug.Log("���ӽ�����");
    }

    // �� �߻��Լ�
    void LaggyFunction()
    {
        Debug.Log("PLC�� �����͸� ��û�մϴ�.");

        // �����δ� ����� �����ɸ��� �˰����� ��
        Thread.Sleep(5000);

        Debug.Log("PLC�κ��� �����͸� ����.");
    }
}
