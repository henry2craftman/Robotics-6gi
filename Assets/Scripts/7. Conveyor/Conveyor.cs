using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conveyor
{
    public class Conveyor : MonoBehaviour
    {
        // �̱���(singleton): �����ִ� ��� ��ũ��Ʈ�� ���� ��ü�� �� �ڽ��� �˼�����.
        public static Conveyor Instance;

        public float speed = 2;
        
        // �����̾� ��Ʈ ����Ʈ
        public List<Sensor> sensors;
        public List<Transform> pushers;
        public List<Transform> dests;

        // ���� ���� ����Ǵ� Lifecycle �޼���
        private void Awake()
        {
            if(Instance == null)
                Instance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void InvokeMovePusher()
        {
            StartCoroutine(MovePusher(pushers[0], dests[0]));
        }

        public void InvokeMovePusher(Sensor sensor)
        {
            //if (sensor == sensors[0])
            //{
            //    StartCoroutine(MovePusher(pushers[0], dests[0]));
            //}
            //else if (sensor == sensors[1])
            //{
            //    StartCoroutine(MovePusher(pushers[1], dests[1]));
            //}

            int index = sensors.IndexOf(sensor);

            StartCoroutine(MovePusher(pushers[index], dests[index]));
        }

        // sensor�� �۵��ϰ� Ư�� �ð� �� ����
        IEnumerator MovePusher(Transform pusher, Transform dest)
        {
            yield return new WaitForSeconds(2);

            Vector3 originPusherPos = pusher.position;

            while(true)
            {
                Vector3 dir = dest.position - pusher.position;
                float distance = dir.magnitude;

                if(distance < 0.1f)
                {
                    pusher.position = originPusherPos;
                    break;
                }

                pusher.position += dir.normalized * speed * Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }
        }
    }


}
