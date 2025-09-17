using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

// 목표 : PLC Data를 갱신하는 Thread와 update 함수(Main Thread)가 공유자원에 Mutex를 사용하여 접근.
public class MutexStudy : MonoBehaviour
{
    object dataLock = new object();    // mutex(잠금장치)
    string latestPlcdata = "";         // 공유자원
    public bool isConnected = false;
    int count = 0;
    

    void Start()
    {
        Debug.Log("백그라운드에서 PLC Data 갱신을 시작합니다.");
        isConnected = true;

        StartCoroutine(CoUpdate());
        Task.Run(UpdatePLCDataAsync);
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
