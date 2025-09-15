using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ��ǥ : ���� �� �� ��ġ�� �����ϰ�, ����� ������ �������� Ư���ӵ��� �̵��Ѵ�.
// �Ӽ� :
namespace MPS
{
    public class Dragger : MonoBehaviour
    {

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(CoMove());
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator CoMove()
        {
            while (true)
            {
                if (Conveyor.Instance.isCWSignal)
                {
                    Vector3 dir = Conveyor.Instance.endPos.position - transform.position;
                    float distance = dir.magnitude;

                    if (distance < 0.1f)
                    {
                        transform.position = Conveyor.Instance.startPos.position;

                    }

                    transform.position += dir.normalized * Conveyor.Instance.speed * Time.deltaTime;
                }
                else if (Conveyor.Instance.isCCWSignal)
                {
                    Vector3 dir = Conveyor.Instance.startPos.position - transform.position;
                    float distance = dir.magnitude;

                    if (distance < 0.1f)
                    {
                        transform.position = Conveyor.Instance.endPos.position;

                    }

                    transform.position += dir.normalized * Conveyor.Instance.speed * Time.deltaTime;
                }
                yield return new WaitForEndOfFrame();
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Plastic" || other.tag == "Metal")
            {
                other.transform.SetParent(this.transform);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Plastic" || other.tag == "Metal")
            {
                other.transform.SetParent(null);
            }
        }
    }
}
 

