using System.Collections;
using UnityEngine;

namespace MPS
{
    // 목표: 시작시 내위치를 저장하고, 명령을 받으면 목적지로 특정 속도로 이동한다.
    public class Dragger : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(CoMove());
        }

        IEnumerator CoMove()
        {
            while (true)
            {
                if (!Conveyor.Instance.isConvOnOffSignal)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

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

                yield return new WaitForSeconds(0.01f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Plastic" || other.tag == "Metal")
            {
                other.transform.SetParent(this.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Plastic" || other.tag == "Metal")
            {
                other.transform.SetParent(null); // 부모로 부터 독립
            }
        }
    }
}
