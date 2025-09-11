using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conveyor
{
    public class Conveyor : MonoBehaviour
    {
        // 싱글턴(singleton): 씬에있는 모든 스크립트를 가진 객체가 나 자신을 알수있음.
        public static Conveyor Instance;

        public float speed = 2;
        
        // 컨베이어 세트 리스트
        public List<Sensor> sensors;
        public List<Transform> pushers;
        public List<Transform> dests;

        // 가장 빨리 실행되는 Lifecycle 메서드
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

        // sensor가 작동하고 특정 시간 후 실행
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
