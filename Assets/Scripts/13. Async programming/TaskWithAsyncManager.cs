using UnityEngine;
using System.Threading.Tasks;

// 목표 : PLC의 데이터를 요청/전송하는 부분만 Task와 Async - Await 함수로 처리
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
        Debug.Log("PLC Data를 불러옵니다.");

        string plcData = await Task.Run(() =>
        {
            Debug.Log("Task 실행");

            Task.Delay(1000);

            Debug.Log("Task 종료");

            return "0000 0000 0000 0001";
        });

        Debug.Log("PLC Data를 받아왔습니다.");
    }
}
