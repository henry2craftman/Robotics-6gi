using UnityEngine;
using System.Threading.Tasks;

// ��ǥ : PLC�� �����͸� ��û/�����ϴ� �κи� Task�� Async - Await �Լ��� ó��
public class TaskWithAsyncManager : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            ReadPLCDataAsync();
        }
    }

    async void ReadPLCDataAsync()
    {
        Debug.Log("PLC Data�� �ҷ��ɴϴ�.");

        string plcData = await Task.Run(() =>
        {
            Debug.Log("Task ����");

            Task.Delay(1000);

            Debug.Log("Task ����");

            return "0000 0000 0000 0001";
        });

        Debug.Log("PLC Data�� �޾ƿԽ��ϴ�.");
    }
}
