using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

// ��ǥ : PLC Data�� �����ϴ� Thread�� update �Լ�(Main Thread)�� �����ڿ��� Mutex�� ����Ͽ� ����.
public class MutexStudy : MonoBehaviour
{
    object dataLock = new object();    // mutex(�����ġ)
    string latestPlcdata = "";         // �����ڿ�
    public bool isConnected = false;
    int count = 0;
    

    void Start()
    {
        Debug.Log("��׶��忡�� PLC Data ������ �����մϴ�.");
        isConnected = true;

        StartCoroutine(CoUpdate());     // 1. ���ν����忡�� ���� �ٲ�
        Task.Run(UpdatePLCDataAsync);   // 2. sub�����忡�� ���� �ٲ�
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(latestPlcdata);
    }

    IEnumerator CoUpdate()
    {
        while(true)
        {
            string updateData = "Update data" + count++;

            lock (dataLock)
            {
                latestPlcdata = updateData;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Task�� ������ async/await���� Unity���� �Լ��� ������� ����(��. Random.Range, Debug.Log)
    async Task UpdatePLCDataAsync()
    {
        while(isConnected)
        {
            string newData = "PLC DATA : " + count++;

            lock (dataLock)
            {
                latestPlcdata = newData;
            }

            await Task.Delay(100);

        }
    }
}
