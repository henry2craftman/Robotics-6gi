using System.Collections;
using UnityEngine;

// ��ǥ: �ڷ�ƾ �Լ��� ����ؼ� ĳ������ �������� �����.
// ������: A -> B�� �̵�
// �Ӽ�: �ӵ�, �����, ������
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
        coroutine = StartCoroutine(CoMove()); // �ڷ�ƾ �Լ� ���۰� ���ÿ� coroutine�� instance ����
    }

    // Update is called once per frame
    void Update()
    {
        print("Update �۵�");

        if(Input.GetKeyDown(KeyCode.Space))
        {
            print("�ڷ�ƾ �Ͻ�����");
            isActive = !isActive;
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            StopCoroutine(coroutine); // coroutine instance ���� �ߴ�
        }
    }

    // �ڷ�ƾ�Լ��� Update�Լ��� �ٸ��� ������ ������ �۵�
    // �ڷ�ƾ �Լ� ���۹��: StartCoroutine ���
    // �ڸ�ƾ �Լ� �ߴܹ��
    // 1. �ߴ� ��ȣ�� �޾�, �ڷ�ƾ �Լ� ���ο��� ���� �Ͻ�����
    // 2. StopCoroutine ���

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

            yield return new WaitUntil(() => isActive == true); // ���ٽ� -> isActive�� true�� �� ���� ��ٸ���.
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
                print("����");
                break;
            }

            transform.position += dir.normalized * speed * Time.deltaTime;

            yield return new WaitForEndOfFrame(); // �������� ���� �� ���� ��ٸ���
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
                print("����");
                break;
            }

            transform.position += dir.normalized * speed * Time.deltaTime;

            yield return new WaitForEndOfFrame(); // �������� ���� �� ���� ��ٸ���
        }
    }
}
