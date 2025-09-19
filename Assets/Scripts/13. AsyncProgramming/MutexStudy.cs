using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

// 목표: PLC 데이터를 갱신하는 스레드와 업데이트 함수(메인 스레드)가 공유자원을 뮤텍스를 사용하여 접근
public class MutexStudy : MonoBehaviour
{
    object dataLock = new object(); // 뮤텍스(잠금장치, 키): 어떤 공유자원에 잠금장치를 적용할것인지 선택
    string latestPlcData = "";      // 공유자원(공유데이터, 공용화장실)
    bool isConnected = false;
    int count = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("백그라운드에서 PLC 데이터 갱신을 시작합니다.");
        isConnected = true;

        StartCoroutine(CoUpdate());     // 1. 메인스레드에서 값을 바꿈
        Task.Run(UpdatePlcDataAsync);   // 2. Sub스레드에서 값을 마꿈
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(latestPlcData);
    }

    IEnumerator CoUpdate()
    {
        while(true)
        {
            string updateData = "Update용 데이터" + count++;

            lock (dataLock) // 뮤텍스
            {
                // latestPlcData: 백그라운드 스레드와 메인스레드가 공유
                latestPlcData = updateData;
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    // Task로 실행한 async/await에는 Unity관련 함수가 실행되지 않음.(예. Random.Range, Debug.Log)
    async void UpdatePlcDataAsync()
    {
        while (isConnected)
        {
            string newData = "PLC 데이터 " + count++;

            lock (dataLock) // 뮤텍스
            {
                latestPlcData = newData;
            }
            
            await Task.Delay(100);
        }
    }
}
