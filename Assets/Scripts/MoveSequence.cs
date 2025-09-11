using System.Collections;
using UnityEngine;

// 목표: 코루틴 함수를 사용해서 캐릭터의 시퀀스를 만든다.
// 시퀀스: A -> B로 이동
// 속성: 속도, 출발지, 목적지
public class MoveSequence : MonoBehaviour
{
    public float speed;
    Vector3 originPos;
    public Transform target;
    public Transform target1;
    public Transform target2;
    public Transform target3;
    Vector3 dir;
    bool isActive = true;
    Coroutine coroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coroutine = StartCoroutine(CoMove()); // 코루틴 함수 시작과 동시에 coroutine에 instance 참조
    }

    // Update is called once per frame
    void Update()
    {
        print("Update 작동");

        if(Input.GetKeyDown(KeyCode.Space))
        {
            print("코루틴 일시정지");
            isActive = !isActive;
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            StopCoroutine(coroutine); // coroutine instance 강제 중단
        }
    }

    // 코루틴함수는 Update함수와 다르게 굉장히 빠르게 작동
    // 코루틴 함수 시작방법: StartCoroutine 사용
    // 코리틴 함수 중단방법
    // 1. 중단 신호를 받아, 코루틴 함수 내부에서 동작 일시정지
    // 2. StopCoroutine 사용

    IEnumerator CoMove()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);

            yield return Move1();

            yield return new WaitForSeconds(1);

            yield return MoveToTarget(target1);


            yield return new WaitForSeconds(1);

            yield return MoveToTarget(target2);

            yield return new WaitForSeconds(1);

            yield return MoveToTarget(target3);

            yield return new WaitUntil(() => isActive == true); // 람다식 -> isActive가 true될 때 까지 기다린다.
        }
    }

    private IEnumerator Move1()
    {
        while (true)
        {
            dir = target.position - transform.position;
            float distance = dir.magnitude;

            if (distance < 0.1f)
            {
                print("멈춤");
                break;
            }

            transform.position += dir.normalized * speed * Time.deltaTime;

            yield return new WaitForEndOfFrame(); // 프레임이 끝날 때 까지 기다리기
        }
    }

    private IEnumerator MoveToTarget(Transform target)
    {
        while (true)
        {
            dir = target.position - transform.position;
            float distance = dir.magnitude;

            if (distance < 0.1f)
            {
                print("멈춤");
                break;
            }

            transform.position += dir.normalized * speed * Time.deltaTime;

            yield return new WaitForEndOfFrame(); // 프레임이 끝날 때 까지 기다리기
        }
    }
}
