using UnityEngine;

// ����: target1���� Ư���ӵ��� �̵��Ѵ�.
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
        // 1. Ÿ�� ���� ���� ã��
        Vector3 dir = end.position - start.position;

        // 2. �Ÿ�Ȯ��
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

        // 3. ũ�Ⱑ 1�� ����
        transform.position += dir.normalized * speed * Time.deltaTime;
    }

    // Lerp ���: A, B��ġ�� Ư�� �ð� ���� �̵�
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

        // Lerp�� t�� 0~1�� �����ؼ� ���ο� Vector�� ���
        // Linear Interpolation(������ȯ)
        Vector3 pos = Vector3.Lerp(start.position, end.position, elapsedTime / duration);

        transform.position = pos;
    }
}
