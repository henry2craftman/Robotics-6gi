using UnityEngine;

namespace Conveyor
{
    public class Sensor : MonoBehaviour
    {
        public bool isSensing = false;

        private void OnTriggerEnter(Collider other)
        {
            print(other.name);
            isSensing = true;

            Conveyor.Instance.InvokeMovePusher(this); // 컨베이어가 어떤 센서인지 알수 있도록 값을 전달
        }

        private void OnTriggerExit(Collider other)
        {
            isSensing = false;
        }
    }

}
