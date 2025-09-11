using UnityEngine;

// 역할: target1으로 특정속도로 이동한다.
public class NPC : MonoBehaviour
{
    public Transform target1;
    public Transform target2;
    public Transform target3;
    public Transform target4;
    public float speed = 2;
    public bool isTarget1 = true;
    public int status = 0; // 0, 1, 2, 3 -> 0
    Transform originPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originPos = transform;
    }

    // Update is called once per frame
    void Update()
    {
        //if(status == 0)
        //{
        //    MoveAToB(transform, target1);
        //}
        //else if(status == 1)
        //{
        //    MoveAToB(transform, target2);
        //}
        //else if (status == 2)
        //{
        //    MoveAToB(transform, target3);
        //}
        //else
        //{
        //    MoveAToB(transform, target4);
        //}

        if (status == 0)
        {
            MoveAToB(target1, target2, 5);
        }
        else if (status == 1)
        {
            MoveAToB(target2, target3, 5);
        }
        else if (status == 2)
        {
            MoveAToB(target3, target4, 5);
        }
        else
        {
            MoveAToB(target4, target1, 5);
        }
    }


    private void MoveAToB(Transform start, Transform end)
    {
        // 1. 타겟 방향 벡터 찾기
        Vector3 dir = end.position - start.position;

        // 2. 거리확인
        float distance = dir.magnitude;
        print(distance);

        if (distance < 0.01f)
        {
            //isTarget1 = !isTarget1;

            status++;

            if (status > 3)
            {
                status = 0;
            }
            return;
        }

        // 3. 크기가 1인 벡터
        transform.position += dir.normalized * speed * Time.deltaTime;
    }

    // Lerp 기능: A, B위치로 특정 시간 동안 이동
    public float duration = 2;
    float elapsedTime = 0;
    private void MoveAToB(Transform start, Transform end, float duration)
    {

        elapsedTime += Time.deltaTime;

        if(elapsedTime > duration)
        {
            status++;

            if (status > 3)
            {
                status = 0;
            }

            elapsedTime = 0;
        }

        // Lerp의 t를 0~1로 조절해서 새로운 Vector를 계산
        // Linear Interpolation(선형변환)
        Vector3 pos = Vector3.Lerp(start.position, end.position, elapsedTime / duration);

        transform.position = pos;
    }
}
