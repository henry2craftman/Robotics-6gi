using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class MutexStudy2 : MonoBehaviour
{
    // 두 개의 공유 자원을 표현하는 lock 객체
    private readonly object lockA = new object();
    private readonly object lockB = new object();

    // 백그라운드 스레드의 로그를 메인 스레드에서 안전하게 출력하기 위한 큐
    private readonly ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();

    private bool isRunning = true;

    void Start()
    {
        Debug.Log("교착상태 시뮬레이션을 시작합니다. 잠시 후 로그 출력이 멈출 것입니다.");

        // 1. 백그라운드 스레드: Lock A -> Lock B 순서로 점유 시도
        Task.Run(BackgroundWorker);

        // 2. 메인 스레드 (코루틴): Lock B -> Lock A 순서로 점유 시도
        StartCoroutine(MainThreadWorker());
    }

    void Update()
    {
        // 큐에 쌓인 로그를 메인 스레드에서 출력
        while (logQueue.TryDequeue(out string message))
        {
            Debug.Log(message);
        }
    }

    // 백그라운드 스레드에서 실행될 메서드
    private async void BackgroundWorker()
    {
        logQueue.Enqueue("[백그라운드] 시작");

        while (isRunning)
        {
            logQueue.Enqueue("[백그라운드] Lock A 점유 시도...");
            lock (lockA)
            {
                logQueue.Enqueue("[백그라운드] >>> Lock A 점유 성공!");

                // 메인 스레드가 Lock B를 잡을 시간을 주기 위해 잠시 대기
                logQueue.Enqueue("[백그라운드] Lock B 점유 시도...");
                lock (lockB) // <-- 여기서 메인 스레드가 lockA를 놓아주길 기다리며 BLOCKED
                {
                    logQueue.Enqueue("[백그라운드] !!! Lock A와 B 모두 점유 성공 !!!");
                }
            }

            await Task.Delay(100);
        }
    }

    // 메인 스레드에서 실행될 코루틴
    private System.Collections.IEnumerator MainThreadWorker()
    {
        Debug.Log("[메인 스레드] 시작");

        while (isRunning)
        {
            Debug.Log("[메인 스레드] Lock B 점유 시도...");
            lock (lockB)
            {
                Debug.Log("[메인 스레드] >>> Lock B 점유 성공!");

                // 백그라운드 스레드가 Lock A를 잡을 시간을 주기 위해 잠시 대기

                Debug.Log("[메인 스레드] Lock A 점유 시도...");
                lock (lockA) // <-- 여기서 백그라운드 스레드가 lockB를 놓아주길 기다리며 BLOCKED
                {
                    Debug.Log("[메인 스레드] !!! Lock A와 B 모두 점유 성공 !!!");
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnDestroy()
    {
        // 게임 오브젝트가 파괴될 때 스레드 종료
        isRunning = false;
    }
}