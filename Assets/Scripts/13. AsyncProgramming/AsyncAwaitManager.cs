using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

// 목표: async/await 기능을 사용하여 비동기 작업 예시 만들기(코루틴과 용도 비교)
public class AsyncAwaitManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(CoCountDown());
        }

        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            GetScoreAsync(); // 서버에 데이터를 요청
        }
    }

    IEnumerator CoCountDown()
    {
        Debug.Log("3...");

        yield return new WaitForSeconds(1);

        Debug.Log("2...");

        yield return new WaitForSeconds(1);

        Debug.Log("1...");

        yield return new WaitForSeconds(1);

        Debug.Log("Go!");
    }

    async void GetScoreAsync()
    {
        int score = await FetchScoreFromServerAsync();

        Debug.Log($"서버에서 받아온 점수는 {score}입니다.");
    }

    // 서버에 요청을 보내고 받을 때 까지 기다리는 함수
    async Task<int> FetchScoreFromServerAsync()
    {
        await Task.Delay(2000);

        return 100;
    }
}
