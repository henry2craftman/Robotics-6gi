using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// 목표: PLC의 데이터를 용청/전송하는 부분만 Task와 async/await 함수로 처리
public class TaskWithAsyncManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            ReadPLCDataAsync();
        }
    }

    async void ReadPLCDataAsync()
    {
        Debug.Log("PLC 데이터 읽기 시작!");

        // 백그라운드 비동기 작업자에게 시키기
        string plcData = await Task.Run(() =>
        {
            Debug.Log("Task 실행!");

            Thread.Sleep(1000);

            Debug.Log("Task 종료!");

            return "0000 0000 0000 0011";
        });

        Debug.Log("PLC 데이터를 잘 받아왔습니다.");
    }
}
